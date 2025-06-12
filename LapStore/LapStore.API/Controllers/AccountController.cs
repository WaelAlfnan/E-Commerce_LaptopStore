using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AccountDTO;
using System.Security.Claims;

namespace LapStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _AccountService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _AccountService = accountService;
            _logger = logger;
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                var result = await _AccountService.Register(registerDTO);
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
                var result = await _AccountService.Login(loginDTO);
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

        // 1. Initiate Google login
        [HttpGet("ExternalLogin")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLogin([FromQuery] string provider, [FromQuery] string returnUrl = null)
        {
            try
            {
                var properties = await _AccountService.GetGoogleAuthPropertiesAsync(provider, returnUrl);
                return Challenge(properties, provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error initiating Google login");
                return StatusCode(500, new { message = "An unexpected error occurred during Google login." });
            }
        }

        // 2. Handle Google callback
        [HttpGet("ExternalLoginCallback")]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            try
            {
                if (remoteError != null)
                {
                    return BadRequest(new { Error = $"Error from external provider: {remoteError}" });
                }

                var result = await _AccountService.HandleGoogleCallbackAsync();
                if (!result.Success)
                {
                    return BadRequest(new { errors = result.Errors });
                }

                return Ok(new { token = result.Token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling Google callback");
                return StatusCode(500, new { message = "An unexpected error occurred during Google authentication." });
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

                var success = await _AccountService.LogoutAsync(username);
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

    }
}