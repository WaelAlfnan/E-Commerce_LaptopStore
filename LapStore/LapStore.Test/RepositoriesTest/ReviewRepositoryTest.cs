using LapStore.DAL.Data.Contexts;
using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LapStore.Test.RepositoriesTest
{
    public class ReviewRepositoryTest
    {
        private readonly DbContextOptions<LapStoreDbContext> _options;

        public ReviewRepositoryTest()
        {
            _options = new DbContextOptionsBuilder<LapStoreDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        [Fact]
        public async Task GetProductReviews_ReturnsReviewsForProduct()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            var productId = 1;
            var userId = 1;
            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 5,
                Text = "Great product",
                Date = DateTime.UtcNow,
                user = new User { UserName = "TestUser" },
                product = new Product { Name = "TestProduct" }
            };

            await context.reviews.AddAsync(review);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetProductReviews(productId);

            // Assert
            Assert.Single(result);
            var retrievedReview = result.First();
            Assert.Equal(userId, retrievedReview.UserId);
            Assert.Equal(productId, retrievedReview.ProductId);
            Assert.Equal(5, retrievedReview.Rate);
            Assert.Equal("Great product", retrievedReview.Text);
            Assert.NotNull(retrievedReview.user);
            Assert.Equal("TestUser", retrievedReview.user.UserName);
        }

        [Fact]
        public async Task GetUserReviews_ReturnsReviewsForUser()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            var userId = 1;
            var productId = 1;
            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 4,
                Text = "Good product",
                Date = DateTime.UtcNow,
                user = new User { UserName = "TestUser" },
                product = new Product { Name = "TestProduct" }
            };

            await context.reviews.AddAsync(review);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetUserReviews(userId);

            // Assert
            Assert.Single(result);
            var retrievedReview = result.First();
            Assert.Equal(userId, retrievedReview.UserId);
            Assert.Equal(productId, retrievedReview.ProductId);
            Assert.Equal(4, retrievedReview.Rate);
            Assert.Equal("Good product", retrievedReview.Text);
            Assert.NotNull(retrievedReview.product);
            Assert.Equal("TestProduct", retrievedReview.product.Name);
        }

        [Fact]
        public async Task GetUserProductReview_ReturnsReview()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            var userId = 1;
            var productId = 1;
            var review = new Review
            {
                UserId = userId,
                ProductId = productId,
                Rate = 5,
                Text = "Excellent product",
                Date = DateTime.UtcNow
            };

            await context.reviews.AddAsync(review);
            await context.SaveChangesAsync();

            // Act
            var result = await repository.GetUserProductReview(userId, productId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(productId, result.ProductId);
            Assert.Equal(5, result.Rate);
            Assert.Equal("Excellent product", result.Text);
        }

        [Fact]
        public async Task GetUserProductReview_ReturnsNull_WhenReviewNotFound()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            // Act
            var result = await repository.GetUserProductReview(1, 1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetProductReviews_ReturnsEmptyList_WhenNoReviews()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            // Act
            var result = await repository.GetProductReviews(1);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetUserReviews_ReturnsEmptyList_WhenNoReviews()
        {
            // Arrange
            using var context = new LapStoreDbContext(_options);
            var repository = new ReviewRepository(context);

            // Act
            var result = await repository.GetUserReviews(1);

            // Assert
            Assert.Empty(result);
        }
    }
} 