using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.ReviewDTOs
{
    public class UpdateReviewDTO
    {
        [Required]
        [Range(1, 5)]
        public int Rate { get; set; }

        public string? Text { get; set; }
    }
} 