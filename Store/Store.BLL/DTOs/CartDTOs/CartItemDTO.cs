using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.CartDTOs
{

    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
} 
