using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

namespace LapStore.Web.ViewModels.ProductVM
{
    public class AddProductVM
    {
        #region Properties

        public int Id { get; set; }
        [Required(ErrorMessage = "{0} is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "{0} must be greater than {1}")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Weight must be a number with up to 2 decimal places")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        #endregion
    }
}