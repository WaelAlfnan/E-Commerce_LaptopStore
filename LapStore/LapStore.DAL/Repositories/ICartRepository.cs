using LapStore.DAL.Data.Entities;

namespace LapStore.DAL.Repositories
{
    public interface ICartRepository : IGenericRepository<Cart>
    {
        Task<Cart?> GetUserCart(int userId);
        Task<Cart?> GetCartWithItems(int cartId);
    }
} 