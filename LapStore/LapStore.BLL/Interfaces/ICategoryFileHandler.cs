using LapStore.BLL.DTOs;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public interface ICategoryFileHandler
    {
        Task<AddCategoryDTO> HandleAddCategoryFileUpload(AddCategoryDTO categoryDto, IFormFile file);

        Task<UpdateCategoryDTO> HandleUpdateCategoryFileUpload(UpdateCategoryDTO categoryDto, IFormFile file);
    }
}