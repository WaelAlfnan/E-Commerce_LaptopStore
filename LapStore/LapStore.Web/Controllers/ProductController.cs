using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using LapStore.Web.ViewModels.ProductVM;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace LapStore.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
        }

        private void SetBearerToken()
        {
            var token = HttpContext.Session.GetString("Token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/product");
            if (response.IsSuccessStatusCode)
            {
                var products = JsonConvert.DeserializeObject<List<GetProductVM>>(await response.Content.ReadAsStringAsync());
                return View(products);
            }

            TempData["Error"] = "Could not retrieve products.";
            return View(new List<GetProductVM>());
        }
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (response.IsSuccessStatusCode)
            {
                var product = JsonConvert.DeserializeObject<GetProductVM>(await response.Content.ReadAsStringAsync());
                return View(product);
            }

            TempData["Error"] = "Product not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // We may need to get categories for dropdown
            var response = await _httpClient.GetAsync("/api/category");
            if (response.IsSuccessStatusCode)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await response.Content.ReadAsStringAsync());
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddProductVM model)
        {
            if (!ModelState.IsValid)
            {
                // Get categories again for the dropdown
                var categoriesResponse = await _httpClient.GetAsync("/api/category");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
                }

                return View(model);
            }

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/product", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);

            // Get categories again for the dropdown
            var catResponse = await _httpClient.GetAsync("/api/category");
            if (catResponse.IsSuccessStatusCode)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await catResponse.Content.ReadAsStringAsync());
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            var product = JsonConvert.DeserializeObject<GetProductVM>(await response.Content.ReadAsStringAsync());
            var updateModel = UpdateProductVM.FromProductVM(product);

            // Get categories for dropdown
            var categoriesResponse = await _httpClient.GetAsync("/api/category");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
            }

            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, UpdateProductVM model)
        {
            if (!ModelState.IsValid)
            {
                // Get categories again for the dropdown
                var categoriesResponse = await _httpClient.GetAsync("/api/category");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
                }

                return View(model);
            }

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/product/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);

            // Get categories again for the dropdown
            var catResponse = await _httpClient.GetAsync("/api/category");
            if (catResponse.IsSuccessStatusCode)
            {
                ViewBag.Categories = JsonConvert.DeserializeObject<List<ViewModels.CategoryVM.GetCategoryVM>>(await catResponse.Content.ReadAsStringAsync());
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"/api/product/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Product not found.";
                return RedirectToAction(nameof(Index));
            }

            var product = JsonConvert.DeserializeObject<GetProductVM>(await response.Content.ReadAsStringAsync());
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"/api/product/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Product deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Could not delete the product.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> ByCategory(int categoryId)
        {
            var response = await _httpClient.GetAsync($"/api/product/category/{categoryId}");
            if (response.IsSuccessStatusCode)
            {
                var products = JsonConvert.DeserializeObject<List<GetProductVM>>(await response.Content.ReadAsStringAsync());

                // Get category name
                var categoryResponse = await _httpClient.GetAsync($"/api/category/{categoryId}");
                if (categoryResponse.IsSuccessStatusCode)
                {
                    var category = JsonConvert.DeserializeObject<ViewModels.CategoryVM.GetCategoryVM>(await categoryResponse.Content.ReadAsStringAsync());
                    ViewBag.CategoryName = category.Name;
                }

                return View("Index", products);
            }

            TempData["Error"] = "Could not retrieve products for this category.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction(nameof(Index));

            var response = await _httpClient.GetAsync($"/api/product/search?query={query}");
            if (response.IsSuccessStatusCode)
            {
                var products = JsonConvert.DeserializeObject<List<GetProductVM>>(await response.Content.ReadAsStringAsync());
                ViewBag.SearchQuery = query;
                return View("Index", products);
            }

            TempData["Error"] = "Could not retrieve search results.";
            return RedirectToAction(nameof(Index));
        }
    }
}