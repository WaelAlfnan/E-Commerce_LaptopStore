using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.DAL.Data.Entities
{
    public class OrderItem
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("order")]
        public int OrderId { get; set; }


        [Key]
        [Column(Order = 1)]
        [ForeignKey("product")]
        public int ProductId { get; set; }


        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }


        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }


        // Navigation Properties
        public virtual Order order { get; set; }
        public virtual Product product { get; set; }


    }
}
