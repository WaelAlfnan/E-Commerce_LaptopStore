using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.Web.ViewModels.CategoryVM
{
    public class UpdateCategoryVM
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

        #region Methods

        public static UpdateCategoryVM FromGetCategoryVM(GetCategoryVM categoryVM)
        {
            return new UpdateCategoryVM
            {
                Id = categoryVM.Id,
                Name = categoryVM.Name,
                Description = categoryVM.Description,
                ImageUrl = categoryVM.ImageUrl,
                ParentCategoryId = categoryVM.ParentCategoryId,
            };
        }
        #endregion
    }
}