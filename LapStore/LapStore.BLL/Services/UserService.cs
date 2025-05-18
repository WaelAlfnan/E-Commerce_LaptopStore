using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.DAL;
using Microsoft.AspNetCore.Identity;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AccountDTO;
using Microsoft.Extensions.Logging;

namespace LapStore.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Address> _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUnitOfWork unitOfWork,
            IGenericRepository<Address> addressRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
        }

        public async Task<AuthResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var user = RegisterDTO.FromRegisterDTO(registerDTO);
                user.Role = UserRole.Customer;

                var result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Registration failed for user {Username}: {Errors}", 
                        registerDTO.UserName, string.Join(", ", errors));
                    return new AuthResult { Success = false, Errors = errors };
                }

                var token = _jwtService.GenerateToken(user);
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", registerDTO.UserName);
                return new AuthResult { Success = false, Errors = new[] { "An unexpected error occurred during registration." } };
            }
        }

        public async Task<AuthResult> Login(LoginDTO loginDTO)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(loginDTO.UserName);
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: User {Username} not found", loginDTO.UserName);
                    return new AuthResult { Success = false, Errors = new[] { "Invalid username or password." } };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login attempt failed for user {Username}: Invalid password", loginDTO.UserName);
                    return new AuthResult { Success = false, Errors = new[] { "Invalid username or password." } };
                }

                var token = _jwtService.GenerateToken(user);
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {Username}", loginDTO.UserName);
                return new AuthResult { Success = false, Errors = new[] { "An unexpected error occurred during login." } };
            }
        }

        public async Task<bool> LogoutAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Logout attempt failed: User {Username} not found", username);
                    return false;
                }

                await _signInManager.SignOutAsync();
                _logger.LogInformation("User {Username} logged out successfully", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {Username}", username);
                return false;
            }
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

                if (!string.IsNullOrEmpty(updateProfileDTO.UserName) && user.UserName != updateProfileDTO.UserName)
                {
                    var existingUser = await _userManager.FindByNameAsync(updateProfileDTO.UserName);
                    if (existingUser != null)
                    {
                        return new Result { Success = false, Message = "Username is already taken." };
                    }
                    user.UserName = updateProfileDTO.UserName;
                }

                if (!string.IsNullOrEmpty(updateProfileDTO.Email) && user.Email != updateProfileDTO.Email)
                {
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

        public async Task<AddressInfoDTO?> GetUserAddressAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                if (user?.address == null)
                {
                    return null;
                }

                return AddressInfoDTO.FromAddress(user.address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving address for user {UserId}", userId);
                return null;
            }
        }

        public async Task<Result> AddAddressAsync(int userId, AddAddressDTO addressDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new Result { Success = false, Message = "User not found." };
                }

                var address = AddAddressDTO.FromAddressDTO(addressDTO);
                await _addressRepository.AddAsync(address);
                await _unitOfWork.CompleteAsync();

                user.AddressId = address.Id;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Address update failed for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                return new Result { Success = true, Message = "Address added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                if (user?.address == null || user.address.Id != addressId)
                {
                    return new Result { Success = false, Message = "Address not found." };
                }

                user.address.Street = addressDTO.Street;
                user.address.City = addressDTO.City;
                user.address.Governorate = addressDTO.Governorate;
                user.address.Country = addressDTO.Country;
                user.address.ZipCode = addressDTO.ZipCode;
                await _unitOfWork.CompleteAsync();

                return new Result { Success = true, Message = "Address updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }
    }
}