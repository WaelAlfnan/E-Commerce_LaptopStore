using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.DTOs.OrderDTOs
{
    public class OrderItemDetailsDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
} 