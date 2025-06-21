using Store.DAL.Data.Entities;

namespace Store.DAL.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetUserCart(int userId);
        Task<Cart?> GetCartWithItems(int cartId);
    }
} 