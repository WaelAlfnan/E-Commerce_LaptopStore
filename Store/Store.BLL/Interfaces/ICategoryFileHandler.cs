using Store.BLL.DTOs;
using Microsoft.AspNetCore.Http;

namespace Store.BLL.Services
{
    public interface ICategoryFileHandler
    {
        Task<AddCategoryDTO> HandleAddCategoryFileUpload(AddCategoryDTO categoryDto, IFormFile file);

        Task<UpdateCategoryDTO> HandleUpdateCategoryFileUpload(UpdateCategoryDTO categoryDto, IFormFile file);
    }
}
