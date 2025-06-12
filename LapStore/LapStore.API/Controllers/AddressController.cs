using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AddressDTO;
using System.Security.Claims;

namespace LapStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AddressController : ControllerBase
    {
        private readonly IAddressService _addressService;
        private readonly ILogger<AccountController> _logger;

        public AddressController(IAddressService addressService, ILogger<AccountController> logger)
        {
            _addressService = addressService;
            _logger = logger;
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

                var address = await _addressService.GetUserAddressAsync(userId);
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

                var result = await _addressService.AddAddressAsync(userId, addressDTO);
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

                var result = await _addressService.UpdateAddressAsync(userId, addressId, addressDTO);
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

    }
}