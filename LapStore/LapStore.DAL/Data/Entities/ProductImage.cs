using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LapStore.DAL.Data.Entities
{
    public class ProductImage
    {
        [Key]
        public int Id { get; set; }
        public string URL { get; set; }


        [ForeignKey("product")]
        [Required]
        public int ProductId { get; set; }

        // Navigation Properties
        public virtual Product product { get; set; }
    }
}
