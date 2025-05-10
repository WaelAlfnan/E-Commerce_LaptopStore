using LapStore.DAL.Data.Entities;

namespace LapStore.DAL.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrders(int userId);
        Task<Order?> GetOrderWithItems(int orderId);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status);
    }
} 