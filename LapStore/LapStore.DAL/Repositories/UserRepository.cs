using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(LapStoreDbContext context) : base(context)
        {
        }

        public async Task<bool> IsEmailExistAsync(string email, int? userId = null)
        {
            var query = _dbSet
                .Where(u => u.Email.ToLower() == email.ToLower());

            if (userId.HasValue)
            {
                query = query.Where(u => u.Id != userId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<bool> IsUserNameExistAsync(string userName, int? userId = null)
        {
            var query = _dbSet
                .Where(u => u.UserName.ToLower() == userName.ToLower());

            if (userId.HasValue)
            {
                query = query.Where(u => u.Id != userId.Value);
            }

            return await query.AnyAsync();
        }

        public async Task<User?> GetUserWithAddressAsync(int userId)
        {
            return await _context.users
                .Include(u => u.address)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithOrdersAsync(int userId)
        {
            return await _context.users
                .Include(u => u.orders)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }

        public async Task<User?> GetUserWithCartAsync(int userId)
        {
            return await _context.users
                .Include(u => u.cart)
                .FirstOrDefaultAsync(u => u.Id == userId);
        }
    }
}