using LapStore.BLL.DTOs.CartDTOs;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.API.Controllers
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

        [HttpGet]
        public async Task<ActionResult<CartDetailsDTO>> GetUserCart()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var cart = await _cartService.GetUserCart(userId);
            return Ok(cart);
        }

        [HttpPost("items")]
        public async Task<ActionResult<CartDetailsDTO>> AddItemToCart(CreateCartItemDTO itemDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var cart = await _cartService.AddItemToCart(userId, itemDto);
            return Ok(cart);
        }

        [HttpPut("items/{productId}")]
        public async Task<ActionResult<CartDetailsDTO>> UpdateCartItem(int productId, UpdateCartItemDTO itemDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var cart = await _cartService.UpdateCartItem(userId, productId, itemDto);
            return Ok(cart);
        }

        [HttpDelete("items/{productId}")]
        public async Task<ActionResult<CartDetailsDTO>> RemoveItemFromCart(int productId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var cart = await _cartService.RemoveItemFromCart(userId, productId);
            return Ok(cart);
        }

        [HttpDelete]
        public async Task<IActionResult> ClearCart()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            await _cartService.ClearCart(userId);
            return NoContent();
        }
    }
} 