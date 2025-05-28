using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AccountDTO;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using LapStore.DAL.Data.Entities;

namespace LapStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IUserService userService, ILogger<AccountController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var result = await _userService.Register(registerDTO);
                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                return StatusCode(500, new { message = "An unexpected error occurred during registration." });
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            try
            {
                var result = await _userService.Login(loginDTO);
                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                return StatusCode(500, new { message = "An unexpected error occurred during login." });
            }
        }

        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            try
            {
                var username = User.FindFirst(ClaimTypes.Name)?.Value;
                if (string.IsNullOrEmpty(username))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                var success = await _userService.LogoutAsync(username);
                if (!success)
                {
                    return BadRequest(new { message = "Failed to logout" });
                }

                return Ok(new { message = "Logged out successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout");
                return StatusCode(500, new { message = "An unexpected error occurred during logout." });
            }
        }

        [Authorize]
        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var userProfile = await _userService.GetUserProfileAsync(userId);
                if (userProfile == null)
                {
                    return NotFound(new { message = "User not found" });
                }

                return Ok(userProfile);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user profile");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the profile." });
            }
        }

        [Authorize]
        [HttpPut("Profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDTO updateProfileDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var result = await _userService.UpdateUserProfileAsync(userId, updateProfileDTO);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user profile");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the profile." });
            }
        }

        [Authorize]
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var result = await _userService.ChangePasswordAsync(userId, changePasswordDTO);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error changing password");
                return StatusCode(500, new { message = "An unexpected error occurred while changing the password." });
            }
        }

        [Authorize]
        [HttpGet("Address")]
        public async Task<IActionResult> GetAddress()
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var address = await _userService.GetUserAddressAsync(userId);
                if (address == null)
                {
                    return NotFound(new { message = "Address not found" });
                }

                return Ok(address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user address");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the address." });
            }
        }

        [Authorize]
        [HttpPost("Address")]
        public async Task<IActionResult> AddAddress([FromBody] AddAddressDTO addressDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var result = await _userService.AddAddressAsync(userId, addressDTO);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address");
                return StatusCode(500, new { message = "An unexpected error occurred while adding the address." });
            }
        }

        [Authorize]
        [HttpPut("Address/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, [FromBody] UpdateAddressDTO addressDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    return Unauthorized(new { message = "Invalid user ID in token" });
                }

                var result = await _userService.UpdateAddressAsync(userId, addressId, addressDTO);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address");
                return StatusCode(500, new { message = "An unexpected error occurred while updating the address." });
            }
        }

        // Admin endpoints
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _userService.GetAllUsersAsync();
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all users");
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving users." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { message = "User not found" });
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred while retrieving the user." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/users/{userId}/role")]
        public async Task<IActionResult> UpdateUserRole(int userId, [FromBody] UpdateUserRoleDTO roleDTO)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentAdminId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }

                // Validate the role
                if (!Enum.IsDefined(typeof(UserRole), roleDTO.NewRole))
                {
                    return BadRequest(new { message = "Invalid role specified" });
                }

                var result = await _userService.UpdateUserRoleAsync(userId, currentAdminId, roleDTO.NewRole);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }

                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating role for user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred while updating the user role." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/{userId}")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int currentAdminId))
                {
                    return Unauthorized(new { message = "Invalid token" });
                }
                var result = await _userService.DeleteUserAsync(userId, currentAdminId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred while deleting the user." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/users/{userId}/disable")]
        public async Task<IActionResult> DisableUser(int userId)
        {
            try
            {
                var result = await _userService.DisableUserAsync(userId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disabling user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred while disabling the user." });
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/users/{userId}/enable")]
        public async Task<IActionResult> EnableUser(int userId)
        {
            try
            {
                var result = await _userService.EnableUserAsync(userId);
                if (!result.Success)
                {
                    return BadRequest(new { message = result.Message });
                }
                return Ok(new { message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enabling user {UserId}", userId);
                return StatusCode(500, new { message = "An unexpected error occurred while enabling the user." });
            }
        }

        [HttpPost("admin/first-admin")]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterFirstAdmin([FromBody] AdminRegisterDTO adminDTO)
        {
            try
            {
                // Check if this is the first admin registration
                if (!await _userService.IsFirstAdminAsync())
                {
                    return BadRequest(new { message = "Admin already exists. Please contact the system administrator for access." });
                }

                var result = await _userService.RegisterFirstAdminAsync(adminDTO);
                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { token = result.Token, message = "First admin registered successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during first admin registration");
                return StatusCode(500, new { message = "An unexpected error occurred during registration." });
            }
        }
    }
}