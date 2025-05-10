using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.ReviewDTOs
{
    public class ReviewDTO
    {
        public int UserId { get; set; }
        public int ProductId { get; set; }
        public int Rate { get; set; }
        public string? Text { get; set; }
        public DateTime Date { get; set; }
    }
} 