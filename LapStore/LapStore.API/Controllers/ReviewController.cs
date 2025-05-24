using LapStore.BLL.DTOs.ReviewDTOs;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LapStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly IReviewService _reviewService;
        private readonly ILogger<ReviewController> _logger;

        public ReviewController(IReviewService reviewService, ILogger<ReviewController> logger)
        {
            _reviewService = reviewService ?? throw new ArgumentNullException(nameof(reviewService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets all reviews for a specific product
        /// </summary>
        /// <param name="productId">The ID of the product</param>
        /// <returns>A list of reviews for the product</returns>
        [HttpGet("product/{productId}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<ReviewDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<ReviewDetailsDTO>>> GetProductReviews(int productId)
        {
            try
            {
                var reviews = await _reviewService.GetProductReviews(productId);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid product ID: {ProductId}", productId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for product {ProductId}", productId);
                return StatusCode(500, "An error occurred while retrieving the reviews");
            }
        }

        /// <summary>
        /// Gets all reviews by a specific user
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <returns>A list of reviews by the user</returns>
        [HttpGet("user/{userId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(IEnumerable<ReviewDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<ReviewDetailsDTO>>> GetUserReviews(int userId)
        {
            try
            {
                var reviews = await _reviewService.GetUserReviews(userId);
                return Ok(reviews);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID: {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting reviews for user {UserId}", userId);
                return StatusCode(500, "An error occurred while retrieving the reviews");
            }
        }

        /// <summary>
        /// Gets a specific review by a user for a product
        /// </summary>
        /// <param name="userId">The ID of the user</param>
        /// <param name="productId">The ID of the product</param>
        /// <returns>The review if found, null otherwise</returns>
        [HttpGet("user/{userId}/product/{productId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReviewDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDetailsDTO>> GetUserProductReview(int userId, int productId)
        {
            try
            {
                var review = await _reviewService.GetUserProductReview(userId, productId);
                if (review == null)
                    return NotFound();

                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID or product ID: {UserId}, {ProductId}", userId, productId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting review for user {UserId} and product {ProductId}", userId, productId);
                return StatusCode(500, "An error occurred while retrieving the review");
            }
        }

        /// <summary>
        /// Creates a new review
        /// </summary>
        /// <param name="userId">The ID of the user creating the review</param>
        /// <param name="reviewDto">The review data</param>
        /// <returns>The created review</returns>
        [HttpPost("user/{userId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReviewDetailsDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ReviewDetailsDTO>> CreateReview(int userId, [FromBody] CreateReviewDTO reviewDto)
        {
            try
            {
                var review = await _reviewService.CreateReview(userId, reviewDto);
                return CreatedAtAction(nameof(GetUserProductReview), 
                    new { userId = review.UserId, productId = review.ProductId }, review);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid review data for user {UserId}", userId);
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Review already exists for user {UserId} and product {ProductId}", 
                    userId, reviewDto.ProductId);
                return Conflict(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating review for user {UserId}", userId);
                return StatusCode(500, "An error occurred while creating the review");
            }
        }

        /// <summary>
        /// Updates an existing review
        /// </summary>
        /// <param name="userId">The ID of the user who wrote the review</param>
        /// <param name="productId">The ID of the product being reviewed</param>
        /// <param name="reviewDto">The updated review data</param>
        /// <returns>The updated review</returns>
        [HttpPut("user/{userId}/product/{productId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(typeof(ReviewDetailsDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ReviewDetailsDTO>> UpdateReview(int userId, int productId, [FromBody] UpdateReviewDTO reviewDto)
        {
            try
            {
                var review = await _reviewService.UpdateReview(userId, productId, reviewDto);
                return Ok(review);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid review data for user {UserId} and product {ProductId}", 
                    userId, productId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating review for user {UserId} and product {ProductId}", 
                    userId, productId);
                return StatusCode(500, "An error occurred while updating the review");
            }
        }

        /// <summary>
        /// Deletes a review
        /// </summary>
        /// <param name="userId">The ID of the user who wrote the review</param>
        /// <param name="productId">The ID of the product being reviewed</param>
        /// <returns>No content if successful</returns>
        [HttpDelete("user/{userId}/product/{productId}")]
        [Authorize(Roles = "Customer")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteReview(int userId, int productId)
        {
            try
            {
                await _reviewService.DeleteReview(userId, productId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid user ID or product ID: {UserId}, {ProductId}", userId, productId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting review for user {UserId} and product {ProductId}", 
                    userId, productId);
                return StatusCode(500, "An error occurred while deleting the review");
            }
        }
    }
} 