using LapStore.BLL.DTOs.ReviewDTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;

namespace LapStore.BLL.Services
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _reviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ReviewService(
            IReviewRepository reviewRepository,
            IProductRepository productRepository,
            IUserRepository userRepository,
            IUnitOfWork unitOfWork)
        {
            _reviewRepository = reviewRepository;
            _productRepository = productRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<ReviewDetailsDTO>> GetProductReviews(int productId)
        {
            var reviews = await _reviewRepository.GetProductReviews(productId);
            return reviews.Select(r => MapToReviewDetailsDTO(r));
        }

        public async Task<IEnumerable<ReviewDetailsDTO>> GetUserReviews(int userId)
        {
            var reviews = await _reviewRepository.GetUserReviews(userId);
            return reviews.Select(r => MapToReviewDetailsDTO(r));
        }

        public async Task<ReviewDetailsDTO?> GetUserProductReview(int userId, int productId)
        {
            var review = await _reviewRepository.GetUserProductReview(userId, productId);
            if (review == null) return null;

            return MapToReviewDetailsDTO(review);
        }

        public async Task<ReviewDetailsDTO> CreateReview(int userId, CreateReviewDTO reviewDto)
        {
            var product =  _productRepository.GetById(reviewDto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found");

            var existingReview = await _reviewRepository.GetUserProductReview(userId, reviewDto.ProductId);
            if (existingReview != null)
                throw new InvalidOperationException("User has already reviewed this product");

            var review = new Review
            {
                UserId = userId,
                ProductId = reviewDto.ProductId,
                Rate = reviewDto.Rate,
                Text = reviewDto.Text,
                Date = DateTime.UtcNow
            };

            await _reviewRepository.AddAsync(review);
            await _unitOfWork.CompleteAsync();

            return MapToReviewDetailsDTO(review);
        }

        public async Task<ReviewDetailsDTO> UpdateReview(int userId, int productId, UpdateReviewDTO reviewDto)
        {
            var review = await _reviewRepository.GetUserProductReview(userId, productId);
            if (review == null)
                throw new ArgumentException("Review not found");

            review.Rate = reviewDto.Rate;
            review.Text = reviewDto.Text;

             _reviewRepository.Update(review);
            await _unitOfWork.CompleteAsync();

            return MapToReviewDetailsDTO(review);
        }

        public async Task DeleteReview(int userId, int productId)
        {
            var review = await _reviewRepository.GetUserProductReview(userId, productId);
            if (review == null)
                throw new ArgumentException("Review not found");

             _reviewRepository.Delete(review);
            await _unitOfWork.CompleteAsync();
        }

        private ReviewDetailsDTO MapToReviewDetailsDTO(Review review)
        {
            if (review == null)
                throw new ArgumentNullException(nameof(review));

            return new ReviewDetailsDTO
            {
                UserId = review.UserId,
                UserName = review.user.UserName,
                ProductId = review.ProductId,
                ProductName = review.product.Name,
                Rate = review.Rate,
                Text = review.Text,
                Date = review.Date
            };
        }
    }
} 