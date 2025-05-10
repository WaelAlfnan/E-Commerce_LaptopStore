using System.ComponentModel.DataAnnotations;

namespace LapStore.Web.ViewModels.ProductVM
{
    public class UpdateProductVM
    {
        #region Properties
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "{0} must be greater than {1}")]
        public decimal Price { get; set; }

        [Range(0.01, Double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        [RegularExpression(@"^\d+(\.\d{1,2})?$", ErrorMessage = "Weight must be a number with up to 2 decimal places")]
        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        public int CategoryId { get; set; }

        #endregion

        #region Methods
        public static UpdateProductVM FromProductVM(GetProductVM productVM)
        {
            if (productVM == null)
                return null;

            return new UpdateProductVM
            {
                Id = productVM.Id,
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                Weight = Math.Round(productVM.Weight, 2), // Round to 2 decimal places
                CategoryId = productVM.CategoryId
            };
        }
        #endregion
    }
}