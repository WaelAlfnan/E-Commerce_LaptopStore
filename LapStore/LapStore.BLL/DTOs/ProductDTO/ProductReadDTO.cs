using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;

namespace LapStore.BLL.DTOs
{
    public class ProductReadDTO
    {
        #region Properties
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:F2}")]
        public decimal Weight { get; set; }

        public int CategoryId { get; set; }

        public string CategoryName { get; set; }

        public List<GetProductImageDTO> Images { get; set; } = new List<GetProductImageDTO>();

        public string MainImageUrl { get; set; }
        #endregion

        #region Methods
        public static ProductReadDTO FromProduct(Product product)
        {
            if (product == null)
                return null;

            var dto = new ProductReadDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Weight = product.Weight,
                CategoryId = product.CategoryId,
                CategoryName = product.category?.Name
            };

            if (product.productImages != null && product.productImages.Any())
            {
                dto.Images = product.productImages
                    .Select(img => new GetProductImageDTO
                    {
                        Id = img.Id,
                        URL = img.URL,
                        ProductId = img.ProductId,
                        IsMain = img.IsMain
                    })
                    .ToList();

                var mainImage = product.productImages.FirstOrDefault(img => img.IsMain);
                dto.MainImageUrl = mainImage?.URL ?? product.productImages.FirstOrDefault()?.URL;
            }

            return dto;
        }
        #endregion
    }

    
}