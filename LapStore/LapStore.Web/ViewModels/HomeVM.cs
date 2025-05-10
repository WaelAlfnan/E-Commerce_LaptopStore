using LapStore.Web.ViewModels.CategoryVM;
using LapStore.Web.ViewModels.ProductVM;

namespace LapStore.Web.ViewModels
{
    public class HomeVM
    {
        public List<GetCategoryVM> Categories { get; set; } = new List<GetCategoryVM>();
        public List<GetProductVM> FeaturedProducts { get; set; } = new List<GetProductVM>();
    }
}