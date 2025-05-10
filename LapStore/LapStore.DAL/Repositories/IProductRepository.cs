using LapStore.DAL.Data.Entities;
namespace LapStore.DAL.Repositories
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<Product>> GetAllProductsWithImagesAsync();
        Product GetProductWithImages(int id);
        Task<Product> GetProductWithImagesAsync(int id);
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<Product> GetProductByNameAsync(string name);
        Task<bool> IsProductNameExistAsync(string productName);

        // Image-related methods
        void RemoveProductImage(ProductImage image);
        Task AddProductImageAsync(ProductImage image);
        Task<ProductImage> GetImageByIdAsync(int imageId);
        Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId);
        Task SetMainImageAsync(int productId, int imageId);
    }
}