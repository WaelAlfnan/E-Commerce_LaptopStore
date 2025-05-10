using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.Web.ViewModels.CategoryVM
{
    public class AddCategoryVM
    {
        #region Properties
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [DisplayName("Category Name")]
        public string Name { get; set; }

        public string? Description { get; set; }

        public string? ImageUrl { get; set; }

        [NotMapped]
        [DisplayName("Category Image")]
        public IFormFile? File { get; set; }

        public int? ParentCategoryId { get; set; }
        #endregion
    }
}