using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace LapStore.Web.ViewModels.CategoryVM
{
    public class GetCategoryVM
    {
        #region Properties
        public int Id { get; set; }

        [DisplayName("Category Name")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public int? ParentCategoryId { get; set; }
        #endregion
    }
}


