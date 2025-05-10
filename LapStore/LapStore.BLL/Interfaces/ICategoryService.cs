using LapStore.DAL.Data.Entities;
using LapStore.BLL.DTOs;

namespace LapStore.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<Category> GetCategoryByNameAsync(string name);
        Task<Category> GetCategoryByIdAsync(int id);
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category> AddCategoryAsync(AddCategoryDTO categoryDto);
        Task<Category> UpdateCategoryAsync(UpdateCategoryDTO categoryDto);
        Task DeleteCategory(int categoryId);
        Task<bool> IsCategoryNameExistAsync(string categoryName, int? categoryId = null);
    }
}