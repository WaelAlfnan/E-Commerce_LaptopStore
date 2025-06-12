using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.ProfileDTO;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace LapStore.BLL.Services
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountService> _logger;
        private readonly IUserClaimService _userClaimService;

        public ProfileService(
            UserManager<User> userManager,
            ILogger<AccountService> logger,
            IUserClaimService userClaimService)
        {
            _userManager = userManager;
            _logger = logger;
            _userClaimService = userClaimService;
        }

        public async Task<UserInfoDTO?> GetUserProfileAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Profile retrieval failed: User with ID {UserId} not found", userId);
                    return null;
                }

                return UserInfoDTO.FromUser(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving profile for user {UserId}", userId);
                return null;
            }
        }

        public async Task<Result> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new Result { Success = false, Message = "User not found." };
                }

                string? oldUsername = null;
                string? oldEmail = null;

                if (!string.IsNullOrEmpty(updateProfileDTO.UserName) && user.UserName != updateProfileDTO.UserName)
                {
                    oldUsername = user.UserName;
                    var existingUser = await _userManager.FindByNameAsync(updateProfileDTO.UserName);
                    if (existingUser != null)
                    {
                        return new Result { Success = false, Message = "Username is already taken." };
                    }
                    user.UserName = updateProfileDTO.UserName;
                }

                if (!string.IsNullOrEmpty(updateProfileDTO.Email) && user.Email != updateProfileDTO.Email)
                {
                    oldEmail = user.Email;
                    var existingUser = await _userManager.FindByEmailAsync(updateProfileDTO.Email);
                    if (existingUser != null)
                    {
                        return new Result { Success = false, Message = "Email is already taken." };
                    }
                    user.Email = updateProfileDTO.Email;
                }

                // Update additional fields if provided
                if (!string.IsNullOrEmpty(updateProfileDTO.FirstName)) user.FirstName = updateProfileDTO.FirstName;
                if (!string.IsNullOrEmpty(updateProfileDTO.LastName)) user.LastName = updateProfileDTO.LastName;
                user.Gender = updateProfileDTO.Gender;
                if (updateProfileDTO.BirthDate != default) user.BirthDate = updateProfileDTO.BirthDate;
                if (!string.IsNullOrEmpty(updateProfileDTO.PhoneNumber)) user.PhoneNumber = updateProfileDTO.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Profile update failed for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                // Update profile claims
                var claimResult = await _userClaimService.UpdateProfileClaimsAsync(userId, user.UserName, user.Email);
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to update profile claims for user {UserId}: {Error}", 
                        userId, claimResult.Message);
                }

                // Update name claim if username changed
                if (oldUsername != null)
                {
                    var nameClaimResult = await _userClaimService.UpdateClaimAsync(userId, ClaimTypes.Name, oldUsername, user.UserName);
                    if (!nameClaimResult.Success)
                    {
                        _logger.LogWarning("Failed to update name claim for user {UserId}: {Error}", 
                            userId, nameClaimResult.Message);
                    }
                }

                // Update email claim if email changed
                if (oldEmail != null)
                {
                    var emailClaimResult = await _userClaimService.UpdateClaimAsync(userId, ClaimTypes.Email, oldEmail, user.Email);
                    if (!emailClaimResult.Success)
                    {
                        _logger.LogWarning("Failed to update email claim for user {UserId}: {Error}", 
                            userId, emailClaimResult.Message);
                    }
                }

                return new Result { Success = true, Message = "Profile updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new Result { Success = false, Message = "User not found." };
                }

                var result = await _userManager.ChangePasswordAsync(
                    user,
                    changePasswordDTO.CurrentPassword,
                    changePasswordDTO.NewPassword
                );

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Password change failed for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                return new Result { Success = true, Message = "Password changed successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

    }
}