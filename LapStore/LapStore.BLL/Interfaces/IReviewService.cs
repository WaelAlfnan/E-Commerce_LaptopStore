using LapStore.BLL.DTOs.ReviewDTOs;

namespace LapStore.BLL.Interfaces
{
    public interface IReviewService
    {
        Task<IEnumerable<ReviewDetailsDTO>> GetProductReviews(int productId);
        Task<IEnumerable<ReviewDetailsDTO>> GetUserReviews(int userId);
        Task<ReviewDetailsDTO?> GetUserProductReview(int userId, int productId);
        Task<ReviewDetailsDTO> CreateReview(int userId, CreateReviewDTO reviewDto);
        Task<ReviewDetailsDTO> UpdateReview(int userId, int productId, UpdateReviewDTO reviewDto);
        Task DeleteReview(int userId, int productId);
    }
} 