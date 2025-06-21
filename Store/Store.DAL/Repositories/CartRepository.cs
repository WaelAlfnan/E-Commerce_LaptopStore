using Store.DAL.Data.Contexts;
using Store.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Store.DAL.Repositories
{
    public class CartRepository : GenericRepository<Cart>, ICartRepository
    {
        private readonly StoreDbContext _context;

        public CartRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Cart?> GetUserCart(int userId)
        {
            return await _context.carts
                .Include(c => c.cartItems)
                    .ThenInclude(ci => ci.product)
                .FirstOrDefaultAsync(c => c.UserId == userId);
        }

        public async Task<Cart?> GetCartWithItems(int cartId)
        {
            return await _context.carts
                .Include(c => c.cartItems)
                    .ThenInclude(ci => ci.product)
                .Include(c => c.user)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }
    }
} 