using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.DTOs.OrderDTOs
{
    
    public class CreateOrderDTO
    {
        [Required]
        public List<CreateOrderItemDTO> OrderItems { get; set; }
    }
} 