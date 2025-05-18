using LapStore.BLL.DTOs;
using LapStore.BLL.Interfaces;
using Microsoft.AspNetCore.Http;

namespace LapStore.BLL.Services
{
    public class CategoryFileHandler : ICategoryFileHandler
    {
        private readonly IFileStorageService _fileStorageService;
        public CategoryFileHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<AddCategoryDTO> HandleAddCategoryFileUpload(AddCategoryDTO categoryDto, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Save the file and update the ImageUrl property
                categoryDto.ImageUrl = await _fileStorageService.SaveCategoryImageAsync(file);
            }

            return categoryDto;
        }

        public async Task<UpdateCategoryDTO> HandleUpdateCategoryFileUpload(UpdateCategoryDTO categoryDto, IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                // Save the file and update the ImageUrl property
                categoryDto.ImageUrl = await _fileStorageService.SaveCategoryImageAsync(file);
            }

            return categoryDto;
        }
    }
}