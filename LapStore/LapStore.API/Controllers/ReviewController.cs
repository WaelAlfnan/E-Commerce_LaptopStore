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

        public ReviewController(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        [HttpGet("product/{productId}")]
        public async Task<ActionResult<IEnumerable<ReviewDetailsDTO>>> GetProductReviews(int productId)
        {
            var reviews = await _reviewService.GetProductReviews(productId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("user")]
        public async Task<ActionResult<IEnumerable<ReviewDetailsDTO>>> GetUserReviews()
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var reviews = await _reviewService.GetUserReviews(userId);
            return Ok(reviews);
        }

        [Authorize]
        [HttpGet("user/product/{productId}")]
        public async Task<ActionResult<ReviewDetailsDTO>> GetUserProductReview(int productId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var review = await _reviewService.GetUserProductReview(userId, productId);
            if (review == null)
                return NotFound();

            return Ok(review);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewDetailsDTO>> CreateReview(CreateReviewDTO reviewDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var review = await _reviewService.CreateReview(userId, reviewDto);
            return CreatedAtAction(nameof(GetUserProductReview), new { productId = review.ProductId }, review);
        }

        [Authorize]
        [HttpPut("product/{productId}")]
        public async Task<ActionResult<ReviewDetailsDTO>> UpdateReview(int productId, UpdateReviewDTO reviewDto)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            var review = await _reviewService.UpdateReview(userId, productId, reviewDto);
            return Ok(review);
        }

        [Authorize]
        [HttpDelete("product/{productId}")]
        public async Task<IActionResult> DeleteReview(int productId)
        {
            var userId = int.Parse(User.FindFirst("UserId")?.Value);
            await _reviewService.DeleteReview(userId, productId);
            return NoContent();
        }
    }
} 