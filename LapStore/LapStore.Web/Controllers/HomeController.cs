using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using LapStore.Web.ViewModels;
using LapStore.Web.ViewModels.ProductVM;
using LapStore.Web.ViewModels.CategoryVM;

namespace LapStore.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HomeController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var HomeVM = new HomeVM();

            // Get all categories
            var categoriesResponse = await _httpClient.GetAsync("/api/category");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var categoriesContent = await categoriesResponse.Content.ReadAsStringAsync();
                HomeVM.Categories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(categoriesContent);
            }
            else
            {
                HomeVM.Categories = new List<GetCategoryVM>();
            }

            // Get featured products (you might want to modify the API endpoint to get featured or latest products)
            var productsResponse = await _httpClient.GetAsync("/api/product");
            if (productsResponse.IsSuccessStatusCode)
            {
                var productsContent = await productsResponse.Content.ReadAsStringAsync();
                HomeVM.FeaturedProducts = JsonConvert.DeserializeObject<List<GetProductVM>>(productsContent);

                // Optionally limit to a smaller number for the homepage
                HomeVM.FeaturedProducts = HomeVM.FeaturedProducts.Take(6).ToList();
            }
            else
            {
                HomeVM.FeaturedProducts = new List<GetProductVM>();
            }

            return View(HomeVM);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorVM { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}