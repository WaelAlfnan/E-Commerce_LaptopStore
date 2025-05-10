using System.ComponentModel.DataAnnotations;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.DTOs.OrderDTOs
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public List<OrderItemDTO> OrderItems { get; set; }
    }
}