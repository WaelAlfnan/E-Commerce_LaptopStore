using Store.DAL.Data.Entities;

namespace Store.DAL.Repositories
{
    public interface IOrderRepository : IGenericRepository<Order>
    {
        Task<IEnumerable<Order>> GetUserOrders(int userId);
        Task<Order?> GetOrderWithItems(int orderId);
        Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status);
    }
} 