using Store.DAL.Data.Entities;
using Store.DAL.Repositories;
using Store.BLL.Interfaces;
using Store.DAL;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Store.BLL.Services
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

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _productRepository.GetAllProductsWithImagesAsync();
        }

        public async Task<bool> IsProductNameExistAsync(string productName)
        {
            return await _productRepository.IsProductNameExistAsync(productName);
        }

        public async Task<Product> AddProductWithImagesAsync(Product product, IEnumerable<IFormFile> images)
        {
            await _productRepository.AddAsync(product);
            await _unitOfWork.CompleteAsync();

            if (images != null)
            {
                foreach (var image in images)
                {
                    await AddProductImageAsync(product.Id, image);
                }
            }

            return product;
        }

        public async Task<Product> UpdateProductWithImagesAsync(Product product, IEnumerable<IFormFile> newImages)
        {
            var existingProduct = await _productRepository.GetProductWithImagesAsync(product.Id);
            if (existingProduct == null)
                throw new KeyNotFoundException("Product not found.");

            // Update fields
            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            existingProduct.Weight = product.Weight;
            existingProduct.CategoryId = product.CategoryId;

            _productRepository.Update(existingProduct);
            await _unitOfWork.CompleteAsync();

            // Add new images if provided
            if (newImages != null)
            {
                foreach (var image in newImages)
                {
                    await AddProductImageAsync(existingProduct.Id, image);
                }
            }

            return existingProduct;
        }

        public async Task DeleteProductWithImagesAsync(int productId)
        {
            var product = await _productRepository.GetProductWithImagesAsync(productId);
            if (product == null)
                throw new KeyNotFoundException("Product not found.");

            // Delete all images (DB and physical)
            var images = product.productImages?.ToList() ?? new List<ProductImage>();
            foreach (var image in images)
            {
                _fileStorageService.DeleteImage(image.URL);
                _productRepository.RemoveProductImage(image);
            }

            _productRepository.Delete(product);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<ProductImage> AddProductImageAsync(int productId, IFormFile imageFile)
        {
            string imageUrl = await _fileStorageService.SaveImageAsync(imageFile, productId);

            // Determine if this should be the main image
            bool isMain = !(await _productRepository.GetProductImagesAsync(productId)).Any();

            var productImage = new ProductImage
            {
                URL = imageUrl,
                ProductId = productId,
                IsMain = isMain
            };

            await _productRepository.AddProductImageAsync(productImage);
            await _unitOfWork.CompleteAsync();

            return productImage;
        }

        public async Task RemoveProductImageAsync(int imageId)
        {
            var image = await _productRepository.GetImageByIdAsync(imageId);
            if (image != null)
            {
                _fileStorageService.DeleteImage(image.URL);
                _productRepository.RemoveProductImage(image);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<IEnumerable<ProductImage>> GetProductImagesAsync(int productId)
        {
            return await _productRepository.GetProductImagesAsync(productId);
        }
    }
}
