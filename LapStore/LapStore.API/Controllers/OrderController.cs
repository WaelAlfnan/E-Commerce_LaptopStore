using LapStore.BLL.DTOs.OrderDTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }
        [Authorize(Roles = "Customer")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetUserOrders()
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

            var orders = await _orderService.GetUserOrders(userId);
            return Ok(orders);
        }
        [Authorize]
        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            try
            {
                var order = await _orderService.GetOrderDetails(orderId);
                return Ok(order);
            }
            catch (ArgumentException ex) when (ex.Message == "Order not found")
            {
                return NotFound(new { message = "Order not found" });
            }
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatus(status);
            return Ok(orders);
        }
        [Authorize(Roles = "Customer")]
        [HttpPost]
        public async Task<ActionResult<OrderDetailsDTO>> CreateOrder(CreateOrderDTO orderDto)
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

            var order = await _orderService.CreateOrder(userId, orderDto);
            return CreatedAtAction(nameof(GetOrderDetails), new { orderId = order.Id }, order);
        }

        [HttpPut("{orderId}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDetailsDTO>> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO statusDto)
        {
            var order = await _orderService.UpdateOrderStatus(orderId, statusDto);
            return Ok(order);
        }
    }
} 