using LapStore.BLL.DTOs.OrderDTOs;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<OrderDetailsDTO>> GetUserOrders(int userId);
        Task<OrderDetailsDTO> GetOrderDetails(int orderId);
        Task<IEnumerable<OrderDetailsDTO>> GetOrdersByStatus(OrderStatus status);
        Task<OrderDetailsDTO> CreateOrder(int userId, CreateOrderDTO orderDto);
        Task<OrderDetailsDTO> UpdateOrderStatus(int orderId, UpdateOrderStatusDTO statusDto);
    }
} 