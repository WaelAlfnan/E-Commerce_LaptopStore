using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AdminUserDTO;
using System.Security.Claims;
using LapStore.DAL.Data.Entities;

namespace LapStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdminUserController : ControllerBase
    {
        private readonly IAdminUserService _adminUserService;
        private readonly ILogger<AccountController> _logger;

        public AdminUserController(IAdminUserService adminUserService, ILogger<AccountController> logger)
        {
            _adminUserService = adminUserService;
            _logger = logger;
        }

        // Admin endpoints
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            try
            {
                var users = await _adminUserService.GetAllUsersAsync();
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
                var user = await _adminUserService.GetUserByIdAsync(userId);
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

                var result = await _adminUserService.UpdateUserRoleAsync(userId, currentAdminId, roleDTO.NewRole);
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
                var result = await _adminUserService.DeleteUserAsync(userId, currentAdminId);
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
                var result = await _adminUserService.DisableUserAsync(userId);
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
                var result = await _adminUserService.EnableUserAsync(userId);
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
                if (!await _adminUserService.IsFirstAdminAsync())
                {
                    return BadRequest(new { message = "Admin already exists. Please contact the system administrator for access." });
                }

                var result = await _adminUserService.RegisterFirstAdminAsync(adminDTO);
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