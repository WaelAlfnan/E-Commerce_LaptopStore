using Store.DAL.Data.Entities;
using Store.DAL.Repositories;
using Store.DAL;
using Microsoft.AspNetCore.Identity;
using Store.BLL.Interfaces;
using Store.BLL.DTOs.AdminUserDTO;
using Store.BLL.DTOs.ProfileDTO;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Store.BLL.Services
{
    public class AdminUserService : IAdminUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountService> _logger;
        private readonly IRoleService _roleService;
        private readonly IUserClaimService _userClaimService;

        public AdminUserService(
            UserManager<User> userManager,
            IJwtService jwtService,
            ILogger<AccountService> logger,
            IRoleService roleService,
            IUserClaimService userClaimService)
        {
            _userManager = userManager;
            _jwtService = jwtService;
            _logger = logger;
            _roleService = roleService;
            _userClaimService = userClaimService;
        }

        public async Task<bool> IsFirstAdminAsync()
        {
            try
            {
                // Check if any user with Admin role exists
                var adminExists = await _userManager.Users
                    .AnyAsync(u => u.Role == UserRole.Admin);

                return !adminExists;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking for first admin");
                throw;
            }
        }

        public async Task<AuthResult> RegisterFirstAdminAsync(AdminRegisterDTO adminDTO)
        {
            try
            {
                // Check if this is the first admin registration
                if (!await IsFirstAdminAsync())
                {
                    _logger.LogWarning("Attempt to register first admin when admin already exists");
                    return new AuthResult 
                    { 
                        Success = false, 
                        Errors = new[] { "Admin already exists. Please contact the system administrator for access." } 
                    };
                }

                // Create the admin user
                var admin = AdminRegisterDTO.FromAdminRegisterDTO(adminDTO);

                // Create the user with password
                var result = await _userManager.CreateAsync(admin, adminDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("First admin registration failed: {Errors}", 
                        string.Join(", ", errors));
                    return new AuthResult { Success = false, Errors = errors };
                }

                // Ensure Admin role exists
                if (!await _roleService.RoleExistsAsync(UserRole.Admin.ToString()))
                {
                    await _roleService.CreateRoleAsync(UserRole.Admin.ToString());
                }

                // Add admin role
                await _userManager.AddToRoleAsync(admin, UserRole.Admin.ToString());

                // Add admin registration claims
                var claimResult = await _userClaimService.AddAdminRegistrationClaimsAsync(admin);
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to add admin registration claims: {Error}", claimResult.Message);
                }

                // Generate JWT token
                var token = await _jwtService.GenerateTokenAsync(admin);

                _logger.LogInformation("First admin registered successfully: {Username}", adminDTO.UserName);
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during first admin registration");
                return new AuthResult 
                { 
                    Success = false, 
                    Errors = new[] { "An unexpected error occurred during registration." } 
                };
            }
        }

        public async Task<IEnumerable<UserInfoDTO>> GetAllUsersAsync()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();
                return users.Select(UserInfoDTO.FromUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return Enumerable.Empty<UserInfoDTO>();
            }
        }

        public async Task<UserInfoDTO?> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                return user != null ? UserInfoDTO.FromUser(user) : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user by id {UserId}", userId);
                return null;
            }
        }

        public async Task<Result> UpdateUserRoleAsync(int userId, int currentAdminId, UserRole newRole)
        {
            try
            {
                if (userId == currentAdminId)
                {
                    return new Result { Success = false, Message = "You cannot change your own admin role." };
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return new Result { Success = false, Message = "User not found." };

                // Ensure the role exists
                if (!await _roleService.RoleExistsAsync(newRole.ToString()))
                {
                    await _roleService.CreateRoleAsync(newRole.ToString());
                }

                // Get current role for claim update
                var currentRole = user.Role.ToString();

                user.Role = newRole;
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                // Update user's role in Identity
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, newRole.ToString());

                // Update role claims
                var claimResult = await _userClaimService.UpdateRoleClaimsAsync(userId, newRole.ToString());
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to update role claims for user {UserId}: {Error}", 
                        userId, claimResult.Message);
                }

                // Update role claim
                var roleClaimResult = await _userClaimService.UpdateClaimAsync(userId, ClaimTypes.Role, currentRole, newRole.ToString());
                if (!roleClaimResult.Success)
                {
                    _logger.LogWarning("Failed to update role claim for user {UserId}: {Error}", 
                        userId, roleClaimResult.Message);
                }

                return new Result { Success = true, Message = "User role updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user role for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> DeleteUserAsync(int userId, int currentAdminId)
        {
            try
            {
                if (userId == currentAdminId)
                {
                    return new Result { Success = false, Message = "You cannot delete your own admin account." };
                }

                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return new Result { Success = false, Message = "User not found." };

                // Get user claims before deletion
                var userClaims = await _userClaimService.GetUserClaimsAsync(userId);
                
                // Remove all user claims
                foreach (var claim in userClaims)
                {
                    var removeResult = await _userClaimService.RemoveClaimAsync(userId, claim.Type, claim.Value);
                    if (!removeResult.Success)
                    {
                        _logger.LogWarning("Failed to remove claim {ClaimType} for user {UserId}: {Error}", 
                            claim.Type, userId, removeResult.Message);
                    }
                }

                // Remove user from all roles
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Any())
                {
                    var roleResult = await _userManager.RemoveFromRolesAsync(user, roles);
                    if (!roleResult.Succeeded)
                    {
                        _logger.LogWarning("Failed to remove roles for user {UserId}: {Errors}", 
                            userId, string.Join(", ", roleResult.Errors.Select(e => e.Description)));
                    }
                }

                // Delete the user
                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("User {UserId} deleted successfully. Removed {ClaimCount} claims and {RoleCount} roles", 
                    userId, userClaims.Count(), roles.Count());
                return new Result { Success = true, Message = "User deleted successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> DisableUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return new Result { Success = false, Message = "User not found." };

                // Lock the user out until a far future date
                var result = await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                // Add disabled claim
                var claimResult = await _userClaimService.AddClaimAsync(userId, "AccountStatus", "Disabled");
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to add disabled claim for user {UserId}: {Error}", 
                        userId, claimResult.Message);
                }

                return new Result { Success = true, Message = "User disabled (locked out) successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> EnableUserAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                    return new Result { Success = false, Message = "User not found." };

                // Remove lockout
                var result = await _userManager.SetLockoutEndDateAsync(user, null);
                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                // Remove disabled claim
                var claimResult = await _userClaimService.RemoveClaimAsync(userId, "AccountStatus", "Disabled");
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to remove disabled claim for user {UserId}: {Error}", 
                        userId, claimResult.Message);
                }

                return new Result { Success = true, Message = "User enabled (lockout removed) successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

    }
}
