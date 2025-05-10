using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace LapStore.DAL.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly LapStoreDbContext _context;

        public ReviewRepository(LapStoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetProductReviews(int productId)
        {
            return await _context.reviews
                .Include(r => r.user)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviews(int userId)
        {
            return await _context.reviews
                .Include(r => r.product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<Review?> GetUserProductReview(int userId, int productId)
        {
            return await _context.reviews
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
} 