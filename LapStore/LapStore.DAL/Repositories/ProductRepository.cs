using LapStore.DAL.Data.Entities;
using LapStore.DAL.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace LapStore.DAL.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        public ProductRepository(LapStoreDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Product>> GetAllProductsWithImagesAsync()
        {
            return await _context.products
                .Include(p => p.productImages)
                .Include(p => p.category)
                .ToListAsync();
        }

        public Product GetProductWithImages(int id)
        {
            return _context.products
                .Include(p => p.productImages)
                .Include(p => p.category)
                .FirstOrDefault(p => p.Id == id);
        }

        public async Task<Product> GetProductWithImagesAsync(int id)
        {
            return await _context.products
                .Include(p => p.productImages)
                .Include(p => p.category)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.products
                .Include(p => p.productImages)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        public async Task<Product> GetProductByNameAsync(string name)
        {
            return await _context.products
                .Include(p => p.productImages)
                .FirstOrDefaultAsync(p => p.Name.ToLower() == name.ToLower());
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            return await _dbSet.AnyAsync(p => p.Name.ToLower() == productName.ToLower());
        }

        public void RemoveProductImage(ProductImage image)
        {
            _context.productImages.Remove(image);
        }

        public async Task AddProductImageAsync(ProductImage image)
        {
            await _context.productImages.AddAsync(image);
        }

        public async Task<ProductImage> GetImageByIdAsync(int imageId)
        {
            return await _context.productImages.FindAsync(imageId);
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _context.productImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();
        }

        public async Task SetMainImageAsync(int productId, int imageId)
        {
            // Reset all images for this product to not be main
            var productImages = await _context.productImages
                .Where(pi => pi.ProductId == productId)
                .ToListAsync();

            foreach (var image in productImages)
            {
                image.IsMain = (image.Id == imageId);
            }

            _context.productImages.UpdateRange(productImages);
        }
    }
}