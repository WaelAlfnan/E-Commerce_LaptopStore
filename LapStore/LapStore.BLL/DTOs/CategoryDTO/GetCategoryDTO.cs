using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.BLL.DTOs
{
    public class GetCategoryDTO
    {
        #region Properties
        public int Id { get; set; }

        [DisplayName("Category Name")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        #endregion

        #region Methods
        public static GetCategoryDTO FromCategory(Category category)
        {
            return new GetCategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        public static Category FromCategoryDTO(GetCategoryDTO category)
        {
            return new Category
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                ImageUrl = category.ImageUrl,
                ParentCategoryId = category.ParentCategoryId,
            };
        }
        #endregion
    }
}
