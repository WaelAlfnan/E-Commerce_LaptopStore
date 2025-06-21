using Store.BLL.Interfaces;
using Store.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Store.BLL.Services
{
    public class UserClaimService : IUserClaimService
    {
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UserClaimService> _logger;

        public UserClaimService(
            UserManager<User> userManager,
            ILogger<UserClaimService> logger)
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<Result> AddClaimAsync(int userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to add claim for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.AddClaimAsync(user, claim);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to add claim for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully added claim {ClaimType} for user {UserId}", 
                    claimType, userId);
                return new Result { Success = true, Message = "Claim added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding claim for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> AddClaimsAsync(int userId, IEnumerable<Claim> claims)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to add claims for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                if (claims == null || !claims.Any())
                {
                    _logger.LogWarning("No claims provided for user {UserId}", userId);
                    return new Result { Success = false, Message = "No claims provided." };
                }

                // Add any existing claims to avoid duplicates
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var newClaims = claims.Where(c => !existingClaims.Any(ec => 
                    ec.Type == c.Type && ec.Value == c.Value));

                if (!newClaims.Any())
                {
                    _logger.LogInformation("No new claims to add for user {UserId}", userId);
                    return new Result { Success = true, Message = "No new claims to add." };
                }

                var result = await _userManager.AddClaimsAsync(user, newClaims);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to add claims for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully added {ClaimCount} claims for user {UserId}", 
                    newClaims.Count(), userId);
                return new Result { Success = true, Message = "Claims added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding claims for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> UpdateClaimAsync(int userId, string claimType, string oldValue, string newValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to update claim for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                var oldClaim = new Claim(claimType, oldValue);
                var newClaim = new Claim(claimType, newValue);

                var result = await _userManager.ReplaceClaimAsync(user, oldClaim, newClaim);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to update claim for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully updated claim {ClaimType} for user {UserId}", 
                    claimType, userId);
                return new Result { Success = true, Message = "Claim updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating claim for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> RemoveClaimAsync(int userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to remove claim for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                var claim = new Claim(claimType, claimValue);
                var result = await _userManager.RemoveClaimAsync(user, claim);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to remove claim for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully removed claim {ClaimType} for user {UserId}", 
                    claimType, userId);
                return new Result { Success = true, Message = "Claim removed successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing claim for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<IEnumerable<Claim>> GetUserClaimsAsync(int userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to get claims for non-existent user: {UserId}", userId);
                    return Enumerable.Empty<Claim>();
                }

                return await _userManager.GetClaimsAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting claims for user {UserId}", userId);
                return Enumerable.Empty<Claim>();
            }
        }

        public async Task<bool> HasClaimAsync(int userId, string claimType, string claimValue)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to check claim for non-existent user: {UserId}", userId);
                    return false;
                }

                var claims = await _userManager.GetClaimsAsync(user);
                return claims.Any(c => c.Type == claimType && c.Value == claimValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking claim for user {UserId}", userId);
                return false;
            }
        }

        public async Task<Result> UpdateProfileClaimsAsync(int userId, string userName, string email)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to update profile claims for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.Email, email)
                };

                // Remove old claims
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var nameClaims = existingClaims.Where(c => c.Type == ClaimTypes.Name);
                var emailClaims = existingClaims.Where(c => c.Type == ClaimTypes.Email);

                foreach (var claim in nameClaims.Concat(emailClaims))
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                // Add new claims
                var result = await _userManager.AddClaimsAsync(user, claims);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to update profile claims for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully updated profile claims for user {UserId}", userId);
                return new Result { Success = true, Message = "Profile claims updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating profile claims for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> UpdateRoleClaimsAsync(int userId, string role)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    _logger.LogWarning("Attempt to update role claims for non-existent user: {UserId}", userId);
                    return new Result { Success = false, Message = "User not found." };
                }

                // Remove existing role claims
                var existingClaims = await _userManager.GetClaimsAsync(user);
                var roleClaims = existingClaims.Where(c => c.Type == ClaimTypes.Role);
                foreach (var claim in roleClaims)
                {
                    await _userManager.RemoveClaimAsync(user, claim);
                }

                // Add new role claim
                var result = await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, role));

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to update role claims for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully updated role claims for user {UserId}", userId);
                return new Result { Success = true, Message = "Role claims updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role claims for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> AddRegistrationClaimsAsync(User user, string registrationSource = "Web")
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim("RegistrationDate", DateTime.UtcNow.ToString("o")),
                    new Claim("RegistrationSource", registrationSource)
                };

                var result = await _userManager.AddClaimsAsync(user, claims);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to add registration claims for user {UserId}: {Errors}", 
                        user.Id, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully added registration claims for user {UserId}", user.Id);
                return new Result { Success = true, Message = "Registration claims added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding registration claims for user {UserId}", user.Id);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> AddAdminRegistrationClaimsAsync(User user)
        {
            try
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                    new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
                    new Claim(ClaimTypes.Role, UserRole.Admin.ToString()),
                    new Claim("RegistrationDate", DateTime.UtcNow.ToString("o")),
                    new Claim("RegistrationSource", "Admin"),
                    new Claim("IsFirstAdmin", "true")
                };

                var result = await _userManager.AddClaimsAsync(user, claims);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to add admin registration claims for user {UserId}: {Errors}", 
                        user.Id, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                _logger.LogInformation("Successfully added admin registration claims for user {UserId}", user.Id);
                return new Result { Success = true, Message = "Admin registration claims added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding admin registration claims for user {UserId}", user.Id);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }
    }
} 
