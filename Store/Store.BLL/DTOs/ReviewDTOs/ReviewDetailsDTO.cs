using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.ReviewDTOs
{
    /// <summary>
    /// Data Transfer Object for detailed review information
    /// </summary>
    public class ReviewDetailsDTO
    {
        /// <summary>
        /// The ID of the user who wrote the review
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// The username of the reviewer
        /// </summary>
        [Required]
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// The ID of the product being reviewed
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// The name of the product being reviewed
        /// </summary>
        [Required]
        public string ProductName { get; set; } = string.Empty;

        /// <summary>
        /// The rating given (1-5)
        /// </summary>
        [Range(1, 5)]
        public int Rate { get; set; }

        /// <summary>
        /// The review text content
        /// </summary>
        public string? Text { get; set; }

        /// <summary>
        /// The date when the review was created
        /// </summary>
        public DateTime Date { get; set; }
    }
} 
