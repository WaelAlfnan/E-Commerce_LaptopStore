using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;

namespace LapStore.Web.ViewModels
{
    public class ProductVM
    {
        #region Properties
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Weight is required")]
        [Range(0.01, Double.MaxValue, ErrorMessage = "Weight must be greater than 0")]
        public decimal Weight { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [DisplayName("Category")]
        public int CategoryId { get; set; }

        [DisplayName("Category")]
        public string? CategoryName { get; set; }
        #endregion

        #region Methods
        public static ProductVM FromProduct(Product product)
        {
            return new ProductVM
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                CategoryId = product.CategoryId,
            };
        }

        public static Product FromProductVM(ProductVM productVM)
        {
            return new Product
            {
                Id = productVM.Id,
                Name = productVM.Name,
                Description = productVM.Description,
                Price = productVM.Price,
                Weight = productVM.Weight,
                CategoryId = productVM.CategoryId,
            };
        }
        #endregion
    }
}