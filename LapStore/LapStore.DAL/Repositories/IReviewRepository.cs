using LapStore.DAL.Data.Entities;

namespace LapStore.DAL.Repositories
{
    public interface IReviewRepository : IGenericRepository<Review>
    {
        Task<IEnumerable<Review>> GetProductReviews(int productId);
        Task<IEnumerable<Review>> GetUserReviews(int userId);
        Task<Review?> GetUserProductReview(int userId, int productId);
    }
} 