using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;

namespace LapStore.BLL.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly string _productUploadsFolder;
        private readonly string _categoryUploadsFolder;
        private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        public FileStorageService(IWebHostEnvironment environment)
        {
            _environment = environment;
            _productUploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "products");
            _categoryUploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "categories");

            // Ensure directories exist
            if (!Directory.Exists(_productUploadsFolder))
            {
                Directory.CreateDirectory(_productUploadsFolder);
            }

            if (!Directory.Exists(_categoryUploadsFolder))
            {
                Directory.CreateDirectory(_categoryUploadsFolder);
            }
        }

        public async Task<string> SaveImageAsync(IFormFile file, int productId)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file", nameof(file));
            }

            ValidateImageFile(file);

            // Create product-specific directory
            var productFolder = Path.Combine(_productUploadsFolder, productId.ToString());
            if (!Directory.Exists(productFolder))
            {
                Directory.CreateDirectory(productFolder);
            }

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(productFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative URL for storage in DB
            return $"/uploads/products/{productId}/{fileName}";
        }

        public async Task<string> SaveCategoryImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("Invalid file", nameof(file));
            }

            ValidateImageFile(file);

            // Generate unique filename
            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var filePath = Path.Combine(_categoryUploadsFolder, fileName);

            // Save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Return the relative URL for storage in DB
            return $"/uploads/categories/{fileName}";
        }

        public void DeleteImage(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new ArgumentException("Invalid path", nameof(relativePath));
            }

            // Convert relative URL to physical path
            var filePath = Path.Combine(_environment.WebRootPath, relativePath.TrimStart('/'));

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

        private void ValidateImageFile(IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!_allowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    $"File type not allowed. Allowed image types are: {string.Join(", ", _allowedImageExtensions)}",
                    nameof(file));
            }

            // Additional validation could be added here (file size limits, etc.)
        }
    }
}   