using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.ReviewDTOs
{
    /// <summary>
    /// Data Transfer Object for creating a new review
    /// </summary>
    public class CreateReviewDTO
    {
        /// <summary>
        /// The ID of the product being reviewed
        /// </summary>
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        /// <summary>
        /// The rating given (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rate { get; set; }

        /// <summary>
        /// The review text content
        /// </summary>
        [StringLength(1000, ErrorMessage = "Review text cannot exceed 1000 characters")]
        public string? Text { get; set; }
    }
} 
