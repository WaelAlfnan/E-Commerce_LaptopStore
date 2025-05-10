using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LapStore.DAL.Data.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public OrderStatus Status { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal TotalAmount { get; set; }

        [ForeignKey("user")]
        [Required]
        public int UserId { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual ICollection<OrderItem>? orderItems { get; set; }
    }
}
