using System.ComponentModel.DataAnnotations;
using Store.DAL.Data.Entities;

namespace Store.BLL.DTOs.OrderDTOs
{
    public class UpdateOrderStatusDTO
    {
        [Required]
        public OrderStatus Status { get; set; }
    }
} 
