using LapStore.BLL.DTOs;
using LapStore.BLL.DTOs.ProductDTO;

namespace LapStore.BLL.ViewModels
{
    public class HomeDTO
    {
        public IEnumerable<GetCategoryDTO> Categories { get; set; }
        public IEnumerable<ProductWriteDTO> NewProducts { get; set; }
        public IEnumerable<ProductWriteDTO> TopSellingProducts { get; set; }
    }
} 