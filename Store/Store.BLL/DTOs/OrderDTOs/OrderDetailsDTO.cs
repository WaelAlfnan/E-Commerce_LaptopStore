using System.ComponentModel.DataAnnotations;
using Store.DAL.Data.Entities;

namespace Store.BLL.DTOs.OrderDTOs
{
    public class OrderDetailsDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public OrderStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public List<OrderItemDetailsDTO> OrderItems { get; set; }
    }
}
 
