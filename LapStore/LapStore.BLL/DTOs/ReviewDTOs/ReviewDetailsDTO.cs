using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.ReviewDTOs
{
    public class ReviewDetailsDTO
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Rate { get; set; }
        public string? Text { get; set; }
        public DateTime Date { get; set; }
    }
} 