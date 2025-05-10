using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly IFileStorageService _fileStorageService;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(
            IProductRepository productRepository,
            IFileStorageService fileStorageService,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _fileStorageService = fileStorageService;
            _unitOfWork = unitOfWork;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            return await _productRepository.GetProductWithImagesAsync(id);
        }

        public Product GetById(int? id)
        {
            if (id == null)
                return null;

            return _productRepository.GetProductWithImages(id.Value);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsWithImagesAsync();
        }

        public async Task AddAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _productRepository.Update(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteAsync(Product product)
        {
            // Get all product images to delete files
            var images = await _productRepository.GetProductImagesAsync(product.Id);

            // Delete physical files first
            foreach (var image in images)
            {
                _fileStorageService.DeleteImage(image.URL);
                _productRepository.RemoveProductImage(image);
            }

            _productRepository.Delete(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            return await _productRepository.IsProductNameExistAsync(productName);
        }

        // Image management methods
        public async Task<ProductImage> AddProductImageAsync(int productId, IFormFile imageFile)
        {
            // Save the physical file and get the URL
            string imageUrl = await _fileStorageService.SaveImageAsync(imageFile, productId);

            // Determine if this should be the main image
            bool isMain = !(await _productRepository.GetProductImagesAsync(productId)).Any();

            // Create image entity
            var productImage = new ProductImage
            {
                URL = imageUrl,
                ProductId = productId,
                IsMain = isMain
            };

            // Save to database
            await _productRepository.AddProductImageAsync(productImage);
            await _unitOfWork.CompleteAsync();

            return productImage;
        }

        public async Task RemoveProductImageAsync(int imageId)
        {
            var image = await _productRepository.GetImageByIdAsync(imageId);
            if (image != null)
            {
                // Delete the physical file
                _fileStorageService.DeleteImage(image.URL);

                // Delete from database
                _productRepository.RemoveProductImage(image);
                await _unitOfWork.CompleteAsync();

                // If this was the main image, set another one as main if available
                if (image.IsMain)
                {
                    var remainingImages = await _productRepository.GetProductImagesAsync(image.ProductId);
                    var firstImage = remainingImages.FirstOrDefault();

                    if (firstImage != null)
                    {
                        await SetMainProductImageAsync(image.ProductId, firstImage.Id);
                    }
                }
            }
        }

        public async Task<ProductImage> GetImageByIdAsync(int imageId)
        {
            return await _productRepository.GetImageByIdAsync(imageId);
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _productRepository.GetProductImagesAsync(productId);
        }

        public async Task SetMainProductImageAsync(int productId, int imageId)
        {
            await _productRepository.SetMainImageAsync(productId, imageId);
            await _unitOfWork.CompleteAsync();
        }
    }
}