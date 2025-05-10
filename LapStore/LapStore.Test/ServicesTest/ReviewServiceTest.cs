using LapStore.BLL.DTOs.ReviewDTOs;
using LapStore.BLL.Interfaces;
using LapStore.BLL.Services;
using LapStore.DAL;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Moq;
using Xunit;

namespace LapStore.Test.ServicesTest
{
    public class ReviewServiceTest
    {
        private readonly Mock<IReviewRepository> _reviewRepositoryMock;
        private readonly Mock<IProductRepository> _productRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly ReviewService _reviewService;

        public ReviewServiceTest()
        {
            _reviewRepositoryMock = new Mock<IReviewRepository>();
            _productRepositoryMock = new Mock<IProductRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();

            _reviewService = new ReviewService(
                _reviewRepositoryMock.Object,
                _productRepositoryMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object
            );
        }

        [Fact]
        public async Task GetProductReviews_ReturnsReviews()
        {
            // Arrange
            var productId = 1;
            var reviews = new List<Review>
            {
                new Review
                {
                    UserId = 1,
                    ProductId = productId,
                    Rate = 5,
                    Text = "Great product",
                    Date = DateTime.UtcNow,
                    user = new User { UserName = "TestUser" },
                    product = new Product { Name = "TestProduct" }
                }
            };

            _reviewRepositoryMock.Setup(r => r.GetProductReviews(productId))
                .ReturnsAsync(reviews);

            // Act
            var result = await _reviewService.GetProductReviews(productId);

            // Assert
            Assert.Single(result);
            var review = result.First();
            Assert.Equal(1, review.UserId);
            Assert.Equal("TestUser", review.UserName);
            Assert.Equal(productId, review.ProductId);
            Assert.Equal("TestProduct", review.ProductName);
            Assert.Equal(5, review.Rate);
            Assert.Equal("Great product", review.Text);
        }

        [Fact]
        public async Task GetUserReviews_ReturnsReviews()
        {
            // Arrange
            var userId = 1;
            var reviews = new List<Review>
            {
                new Review
                {
                    UserId = userId,
                    ProductId = 1,
                    Rate = 4,
                    Text = "Good product",
                    Date = DateTime.UtcNow,
                    user = new User { UserName = "TestUser" },
                    product = new Product { Name = "TestProduct" }
                }
            };

            _reviewRepositoryMock.Setup(r => r.GetUserReviews(userId))
                .ReturnsAsync(reviews);

            // Act
            var result = await _reviewService.GetUserReviews(userId);

            // Assert
            Assert.Single(result);
            var review = result.First();
            Assert.Equal(userId, review.UserId);
            Assert.Equal("TestUser", review.UserName);
            Assert.Equal(1, review.ProductId);
            Assert.Equal("TestProduct", review.ProductName);
            Assert.Equal(4, review.Rate);
            Assert.Equal("Good product", review.Text);
        }

        [Fact]
        public async Task GetUserProductReview_ReturnsReview()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 5,
                Text = "Excellent product",
                Date = DateTime.UtcNow,
                user = new User { UserName = "TestUser" },
                product = new Product { Name = "TestProduct" }
            };

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync(review);

            // Act
            var result = await _reviewService.GetUserProductReview(userId, productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("TestUser", result.UserName);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("TestProduct", result.ProductName);
            Assert.Equal(5, result.Rate);
            Assert.Equal("Excellent product", result.Text);
        }

        [Fact]
        public async Task CreateReview_SuccessfullyCreatesReview()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var createReviewDto = new CreateReviewDTO
            {
                ProductId = productId,
                Rate = 5,
                Text = "New review"
            };

            var product = new Product { Id = productId, Name = "TestProduct" };
            var user = new User { Id = userId, UserName = "TestUser" };

            _productRepositoryMock.Setup(r => r.GetById(productId))
                .Returns(product);

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync((Review)null);

            // Act
            var result = await _reviewService.CreateReview(userId, createReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("TestUser", result.UserName);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("TestProduct", result.ProductName);
            Assert.Equal(5, result.Rate);
            Assert.Equal("New review", result.Text);

            _reviewRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Review>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateReview_ThrowsException_WhenProductNotFound()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var createReviewDto = new CreateReviewDTO
            {
                ProductId = productId,
                Rate = 5,
                Text = "New review"
            };

            _productRepositoryMock.Setup(r => r.GetById(productId))
                .Returns((Product)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _reviewService.CreateReview(userId, createReviewDto));
        }

        [Fact]
        public async Task CreateReview_ThrowsException_WhenReviewAlreadyExists()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var createReviewDto = new CreateReviewDTO
            {
                ProductId = productId,
                Rate = 5,
                Text = "New review"
            };

            var product = new Product { Id = productId, Name = "TestProduct" };
            var existingReview = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 4,
                Text = "Existing review"
            };

            _productRepositoryMock.Setup(r => r.GetById(productId))
                .Returns(product);

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync(existingReview);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _reviewService.CreateReview(userId, createReviewDto));
        }

        [Fact]
        public async Task UpdateReview_SuccessfullyUpdatesReview()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var updateReviewDto = new UpdateReviewDTO
            {
                Rate = 4,
                Text = "Updated review"
            };

            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 5,
                Text = "Original review",
                Date = DateTime.UtcNow,
                user = new User { UserName = "TestUser" },
                product = new Product { Name = "TestProduct" }
            };

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync(review);

            // Act
            var result = await _reviewService.UpdateReview(userId, productId, updateReviewDto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal("TestUser", result.UserName);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal("TestProduct", result.ProductName);
            Assert.Equal(4, result.Rate);
            Assert.Equal("Updated review", result.Text);

            _reviewRepositoryMock.Verify(r => r.Update(It.IsAny<Review>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateReview_ThrowsException_WhenReviewNotFound()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var updateReviewDto = new UpdateReviewDTO
            {
                Rate = 4,
                Text = "Updated review"
            };

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync((Review)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _reviewService.UpdateReview(userId, productId, updateReviewDto));
        }

        [Fact]
        public async Task DeleteReview_SuccessfullyDeletesReview()
        {
            // Arrange
            var userId = 1;
            var productId = 1;
            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 5,
                Text = "Review to delete"
            };

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync(review);

            // Act
            await _reviewService.DeleteReview(userId, productId);

            // Assert
            _reviewRepositoryMock.Verify(r => r.Delete(It.IsAny<Review>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.CompleteAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteReview_ThrowsException_WhenReviewNotFound()
        {
            // Arrange
            var userId = 1;
            var productId = 1;

            _reviewRepositoryMock.Setup(r => r.GetUserProductReview(userId, productId))
                .ReturnsAsync((Review)null);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _reviewService.DeleteReview(userId, productId));
        }
    }
} 