using Store.DAL.Data.Entities;

namespace Store.DAL.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetProductReviews(int productId);
        Task<IEnumerable<Review>> GetUserReviews(int userId);
        Task<Review?> GetUserProductReview(int userId, int productId);
    }
} 