using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace LapStore.BLL.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> SaveImageAsync(IFormFile file, int productId);
        Task<string> SaveCategoryImageAsync(IFormFile file);
        void DeleteImage(string relativePath);
    }
}