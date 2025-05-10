using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LapStore.BLL.Interfaces
{
    public interface IProductService
    {
        Task<Product> GetProductByIdAsync(int id);
        Product GetById(int? id);
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<bool> IsProductNameExistAsync(string productName);

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(Product product);

        // Image-related methods
        Task<ProductImage> AddProductImageAsync(int productId, IFormFile imageFile);
        Task RemoveProductImageAsync(int imageId);
        Task<ProductImage> GetImageByIdAsync(int imageId);
        Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);
        Task SetMainProductImageAsync(int productId, int imageId);
    }
}