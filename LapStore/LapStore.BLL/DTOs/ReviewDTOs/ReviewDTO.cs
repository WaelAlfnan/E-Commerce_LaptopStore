using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.ReviewDTOs
{
    /// <summary>
    /// Base Data Transfer Object for review information
    /// </summary>
    public class ReviewDTO
    {
        /// <summary>
        /// The ID of the user who wrote the review
        /// </summary>
        [Required]
        public int UserId { get; set; }

        /// <summary>
        /// The ID of the product being reviewed
        /// </summary>
        [Required]
        public int ProductId { get; set; }

        /// <summary>
        /// The rating given (1-5)
        /// </summary>
        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5")]
        public int Rate { get; set; }

        /// <summary>
        /// The review text content
        /// </summary>
        [StringLength(1000, ErrorMessage = "Review text cannot exceed 1000 characters")]
        public string? Text { get; set; }

        /// <summary>
        /// The date when the review was created
        /// </summary>
        [Required]
        public DateTime Date { get; set; }
    }
} 