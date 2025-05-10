using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.CartDTOs
{


    public class UpdateCartItemDTO
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

} 