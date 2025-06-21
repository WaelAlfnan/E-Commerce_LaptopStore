using Store.BLL.DTOs.CartDTOs;
using Store.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Store.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }


        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetUserCart()
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

            var cart = await _cartService.GetUserCart(userId);
            return Ok(cart);
        }


        [Authorize(Roles = "Customer")]
        [HttpPost("items")]
        public async Task<ActionResult<CartDetailsDTO>> AddItemToCart(CreateCartItemDTO itemDto)
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

            var cart = await _cartService.AddItemToCart(userId, itemDto);
            return Ok(cart);
        }


        [Authorize(Roles = "Customer")]
        [HttpPut("items/{productId}")]
        public async Task<ActionResult<CartDetailsDTO>> UpdateCartItem(int productId, UpdateCartItemDTO itemDto)
        {
            var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(new { message = "Invalid token" });
            }

            if (!int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized(new { message = "Invalid user ID in token" });
            }
            var cart = await _cartService.UpdateCartItem(userId, productId, itemDto);
            return Ok(cart);
        }


        [Authorize(Roles = "Customer")]
        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<CartDetailsDTO>> RemoveItemFromCart(int productId)
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

            var cart = await _cartService.RemoveItemFromCart(userId, productId);
            return Ok(cart);
        }


        [Authorize(Roles = "Customer")]
        [HttpDelete]
        public async Task<IActionResult> ClearCart()
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

            await _cartService.ClearCart(userId);
            return NoContent();
        }
    }
} 
