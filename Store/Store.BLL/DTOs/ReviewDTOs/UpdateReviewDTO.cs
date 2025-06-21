using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.ReviewDTOs
{
    /// <summary>
    /// Data Transfer Object for updating an existing review
    /// </summary>
    public class UpdateReviewDTO
    {
        /// <summary>
        /// The updated rating (1-5)
        /// </summary>
        [Required(ErrorMessage = "Rating is required")]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rate { get; set; }

        /// <summary>
        /// The updated review text content
        /// </summary>
        [StringLength(1000, ErrorMessage = "Review text cannot exceed 1000 characters")]
        public string? Text { get; set; }
    }
} 
