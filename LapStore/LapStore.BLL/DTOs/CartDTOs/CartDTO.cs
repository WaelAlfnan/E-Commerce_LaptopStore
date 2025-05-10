using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.CartDTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public List<CartItemDTO> CartItems { get; set; }
    }
} 