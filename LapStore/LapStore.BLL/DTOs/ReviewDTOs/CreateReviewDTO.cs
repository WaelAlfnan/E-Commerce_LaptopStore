using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.ReviewDTOs
{
    public class CreateReviewDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }

        public string? Text { get; set; }
    }
} 