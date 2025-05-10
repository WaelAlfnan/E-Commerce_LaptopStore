using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.BLL.DTOs
{
    public class UpdateCategoryDTO
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

        public static Category FromCategoryDTO(UpdateCategoryDTO categoryDTO)
        {
            return new Category
            {
                Id = categoryDTO.Id,
                Name = categoryDTO.Name,
                Description = categoryDTO.Description,
                ImageUrl = categoryDTO.ImageUrl,
                ParentCategoryId = categoryDTO.ParentCategoryId,
            };
        }
        #endregion
    }
}