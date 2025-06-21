using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.CartDTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
} 
