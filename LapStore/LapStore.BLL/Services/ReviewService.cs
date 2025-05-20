using LapStore.BLL.DTOs.ReviewDTOs;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace LapStore.BLL.Services
{
    /// <summary>
    /// Service for managing product reviews
    /// </summary>
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
            _reviewRepository = reviewRepository ?? throw new ArgumentNullException(nameof(reviewRepository));
            _productRepository = productRepository ?? throw new ArgumentNullException(nameof(productRepository));
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<IEnumerable<ReviewDetailsDTO>> GetProductReviews(int productId)
        {
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));

            var reviews = await _reviewRepository.GetProductReviews(productId);
            return reviews.Select(MapToReviewDetailsDTO);
        }

        public async Task<IEnumerable<ReviewDetailsDTO>> GetUserReviews(int userId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));

            var reviews = await _reviewRepository.GetUserReviews(userId);
            return reviews.Select(MapToReviewDetailsDTO);
        }

        public async Task<ReviewDetailsDTO?> GetUserProductReview(int userId, int productId)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));

            var review = await _reviewRepository.GetUserProductReview(userId, productId);
            return review != null ? MapToReviewDetailsDTO(review) : null;
        }

        public async Task<ReviewDetailsDTO> CreateReview(int userId, CreateReviewDTO reviewDto)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));
            if (reviewDto == null)
                throw new ArgumentNullException(nameof(reviewDto));

            var product = await _productRepository.GetByIdAsync(reviewDto.ProductId);
            if (product == null)
                throw new ArgumentException("Product not found", nameof(reviewDto.ProductId));

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

            // Reload the review with navigation properties loaded
            var createdReview = await _reviewRepository.GetUserProductReview(userId, reviewDto.ProductId);

            return MapToReviewDetailsDTO(createdReview);
        }

        public async Task<ReviewDetailsDTO> UpdateReview(int userId, int productId, UpdateReviewDTO reviewDto)
        {
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));
            if (reviewDto == null)
                throw new ArgumentNullException(nameof(reviewDto));

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
            if (userId <= 0)
                throw new ArgumentException("Invalid user ID", nameof(userId));
            if (productId <= 0)
                throw new ArgumentException("Invalid product ID", nameof(productId));

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

            if (review.user == null || review.product == null)
                throw new InvalidOperationException("Review navigation properties are not loaded");

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