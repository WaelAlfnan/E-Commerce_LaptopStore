using LapStore.DAL.Data.Entities;
using LapStore.DAL.Repositories;
using LapStore.BLL.Interfaces;
using LapStore.DAL;
using Microsoft.EntityFrameworkCore;
using LapStore.BLL.DTOs;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFileStorageService _fileStorageService;

        public CategoryService(
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork,
            IFileStorageService fileStorageService)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
            _fileStorageService = fileStorageService;
        }

        public async Task<Category> GetCategoryByNameAsync(string name)
        {
            return await _categoryRepository.GetCategoryByNameAsync(name);
        }

        public async Task<Category> GetCategoryByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await _categoryRepository.GetAllAsync();
        }

        public async Task<Category> AddCategoryAsync(AddCategoryDTO categoryDto)
        {
            // Check if category name already exists
            if (await IsCategoryNameExistAsync(categoryDto.Name))
            {
                throw new InvalidOperationException($"A category with the name '{categoryDto.Name}' already exists.");
            }

            var category = AddCategoryDTO.FromCategoryDTO(categoryDto);

            // Handle file upload if provided
            if (categoryDto.File != null && categoryDto.File.Length > 0)
            {
                category.ImageUrl = await _fileStorageService.SaveCategoryImageAsync(categoryDto.File);
            }

            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    await _categoryRepository.AddAsync(category);
                    await _unitOfWork.CompleteAsync();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    // If there was an error and we uploaded an image, delete it
                    if (!string.IsNullOrEmpty(category.ImageUrl))
                    {
                        _fileStorageService.DeleteImage(category.ImageUrl);
                    }

                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            });

            return category;
        }

        public async Task<Category> UpdateCategoryAsync(UpdateCategoryDTO categoryDto)
        {
            // Check if category name already exists (excluding current category)
            if (await IsCategoryNameExistAsync(categoryDto.Name, categoryDto.Id))
            {
                throw new InvalidOperationException($"A category with the name '{categoryDto.Name}' already exists.");
            }

            // Get the existing category
            var existingCategory = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (existingCategory == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryDto.Id} not found.");
            }

            // Keep track of the old image URL
            string oldImageUrl = existingCategory.ImageUrl;

            // Update category properties
            existingCategory.Name = categoryDto.Name;
            existingCategory.Description = categoryDto.Description;
            existingCategory.ParentCategoryId = categoryDto.ParentCategoryId;

            // Handle file upload if provided
            if (categoryDto.File != null && categoryDto.File.Length > 0)
            {
                existingCategory.ImageUrl = await _fileStorageService.SaveCategoryImageAsync(categoryDto.File);
            }
            else
            {
                // If no new file, keep the existing image URL or update it if provided in the DTO
                existingCategory.ImageUrl = !string.IsNullOrEmpty(categoryDto.ImageUrl)
                    ? categoryDto.ImageUrl
                    : oldImageUrl;
            }

            var strategy = _unitOfWork.Context.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();
                    _categoryRepository.Update(existingCategory);
                    await _unitOfWork.CompleteAsync();

                    // If update succeeded and we have a new image, delete the old one
                    if (categoryDto.File != null && !string.IsNullOrEmpty(oldImageUrl))
                    {
                        _fileStorageService.DeleteImage(oldImageUrl);
                    }

                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception)
                {
                    await _unitOfWork.RollbackTransactionAsync();

                    // If there was an error and we uploaded a new image, delete it
                    if (categoryDto.File != null && existingCategory.ImageUrl != oldImageUrl)
                    {
                        _fileStorageService.DeleteImage(existingCategory.ImageUrl);
                    }

                    throw;
                }
            });

            return existingCategory;
        }

        public async Task DeleteCategory(int categoryId)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryId);
            if (category == null)
            {
                throw new InvalidOperationException($"Category with ID {categoryId} not found.");
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Check if the category has any products
                if ((category.products ?? Enumerable.Empty<Product>()).Any())
                {
                    throw new InvalidOperationException("Cannot delete a category that has products. Please ensure the category is empty before attempting to delete it.");
                }

                // Check if the category has any child categories
                if ((category.childCategories ?? Enumerable.Empty<Category>()).Any())
                {
                    throw new InvalidOperationException("Cannot delete a category that has child categories. Please delete or reassign the child categories first.");
                }

                // Store image URL for deletion after the database operation succeeds
                string imageUrl = category.ImageUrl;

                // Delete from database
                _categoryRepository.Delete(category);
                await _unitOfWork.CompleteAsync();

                // Delete the image file if it exists
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    _fileStorageService.DeleteImage(imageUrl);
                }

                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }

        public async Task<bool> IsCategoryNameExistAsync(string categoryName, int? categoryId = null)
        {
            return await _categoryRepository.IsCategoryNameExistAsync(categoryName, categoryId);
        }
    }
}