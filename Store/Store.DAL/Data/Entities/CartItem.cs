using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.DAL.Data.Entities
{
    public class CartItem
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("cart")]
        public int CartId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("product")]
        public int ProductId { get; set; }

        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }

        // Navigation Properties
        public virtual Cart cart { get; set; }
        public virtual Product product { get; set; }
    }
}
