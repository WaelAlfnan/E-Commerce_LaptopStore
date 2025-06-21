using Store.DAL.Data.Contexts;
using Store.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Store.DAL.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly StoreDbContext _context;

        public OrderRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetUserOrders(int userId)
        {
            return await _context.orders
                .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.product)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }

        public async Task<Order?> GetOrderWithItems(int orderId)
        {
            return await _context.orders
                .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.product)
                        .ThenInclude(p => p.productImages)
                .Include(o => o.user)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

        public async Task<IEnumerable<Order>> GetOrdersByStatus(OrderStatus status)
        {
            return await _context.orders
                .Include(o => o.orderItems)
                    .ThenInclude(oi => oi.product)
                .Include(o => o.user)
                .Where(o => o.Status == status)
                .OrderByDescending(o => o.Date)
                .ToListAsync();
        }
    }
} 