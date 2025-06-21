using Store.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Store.BLL.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<bool> IsProductNameExistAsync(string productName);

        Task<Product> AddProductWithImagesAsync(Product product, IEnumerable<IFormFile> images);
        Task<Product> UpdateProductWithImagesAsync(Product product, IEnumerable<IFormFile> newImages);
        Task DeleteProductWithImagesAsync(int productId);

        Task<ProductImage> AddProductImageAsync(int productId, IFormFile imageFile);
        Task RemoveProductImageAsync(int imageId);
        Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);
    }
}
