using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.CartDTOs
{

    public class CreateCartItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

} 
