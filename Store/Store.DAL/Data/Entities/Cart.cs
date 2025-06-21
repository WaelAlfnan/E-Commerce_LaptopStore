using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Store.DAL.Data.Entities
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("user")]
        [Required]
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual ICollection<CartItem>? cartItems { get; set; }
    }
}
