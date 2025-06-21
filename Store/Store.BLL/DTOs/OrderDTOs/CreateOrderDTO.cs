using System.ComponentModel.DataAnnotations;
using Store.DAL.Data.Entities;

namespace Store.BLL.DTOs.OrderDTOs
{
    
    public class CreateOrderDTO
    {
        [Required]
        public List<CreateOrderItemDTO> OrderItems { get; set; }
    }
} 
