using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.CartDTOs
{

    public class CartItemDTO
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
} 