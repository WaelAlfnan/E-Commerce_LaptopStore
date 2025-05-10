using LapStore.BLL.DTOs.OrderDTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetUserOrders()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var orders = await _orderService.GetUserOrders(userId);
            return Ok(orders);
        }

        [HttpGet("{orderId}")]
        public async Task<ActionResult<OrderDetailsDTO>> GetOrderDetails(int orderId)
        {
            var order = await _orderService.GetOrderDetails(orderId);
            return Ok(order);
        }

        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDetailsDTO>>> GetOrdersByStatus(OrderStatus status)
        {
            var orders = await _orderService.GetOrdersByStatus(status);
            return Ok(orders);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDetailsDTO>> CreateOrder(CreateOrderDTO orderDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
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