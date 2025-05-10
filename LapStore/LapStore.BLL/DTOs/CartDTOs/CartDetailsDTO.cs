using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.CartDTOs
{


    public class CartDetailsDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<CartItemDetailsDTO> CartItems { get; set; }
        public decimal TotalAmount { get; set; }
    }

    
} 