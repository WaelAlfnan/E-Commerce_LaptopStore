using System.ComponentModel.DataAnnotations;
using Store.DAL.Data.Entities;

namespace Store.BLL.DTOs.OrderDTOs
{
    public class CreateOrderItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }
} 
