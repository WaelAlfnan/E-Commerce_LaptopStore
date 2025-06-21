using Store.DAL.Data.Contexts;
using Store.DAL.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Store.DAL.Repositories
{
    public class ReviewRepository : GenericRepository<Review>, IReviewRepository
    {
        private readonly StoreDbContext _context;

        public ReviewRepository(StoreDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Review>> GetProductReviews(int productId)
        {
            return await _context.reviews
                .Include(r => r.user)
                .Include(r => r.product)
                .Where(r => r.ProductId == productId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Review>> GetUserReviews(int userId)
        {
            return await _context.reviews
                .Include(r => r.user)
                .Include(r => r.product)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.Date)
                .ToListAsync();
        }

        public async Task<Review?> GetUserProductReview(int userId, int productId)
        {
            return await _context.reviews
                .Include(r => r.user)
                .Include(r => r.product)
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ProductId == productId);
        }
    }
} 