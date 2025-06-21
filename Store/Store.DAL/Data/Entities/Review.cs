namespace Store.DAL.Data.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Review
    {
        [Key]
        [Column(Order = 0)]
        [ForeignKey("user")]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [ForeignKey("product")]
        public int ProductId { get; set; }

        [Range(1, 5)]
        public int Rate { get; set; }

        public string? Text { get; set; }

        public DateTime Date { get; set; }

        // Navigation Properties
        public virtual User user { get; set; }
        public virtual Product product { get; set; }
    }
}
