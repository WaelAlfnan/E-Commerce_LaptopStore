using LapStore.DAL.Data.Entities;

namespace LapStore.DAL.Repositories
{
    public interface IUserRepository : IGenericRepository<User>
    {
        // Keep only methods that add value beyond what Identity provides
        Task<User?> GetUserWithAddressAsync(int userId);
        Task<User?> GetUserWithOrdersAsync(int userId);
        Task<User?> GetUserWithCartAsync(int userId);

        // These methods can be useful alongside Identity
        Task<bool> IsEmailExistAsync(string email, int? userId = null);
        Task<bool> IsUserNameExistAsync(string userName, int? userId = null);
    }
}