using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.OrderDTOs
{
    public class OrderItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

} 
