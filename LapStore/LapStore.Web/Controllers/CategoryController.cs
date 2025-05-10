using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text;
using LapStore.Web.ViewModels.CategoryVM;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;

namespace LapStore.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public CategoryController(IHttpClientFactory httpClientFactory, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
            _webHostEnvironment = webHostEnvironment;
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
            var response = await _httpClient.GetAsync("/api/category");
            if (response.IsSuccessStatusCode)
            {
                var categories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await response.Content.ReadAsStringAsync());
                return View(categories);
            }

            TempData["Error"] = "Could not retrieve categories.";
            return View(new List<GetCategoryVM>());
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"/api/category/{id}");
            if (response.IsSuccessStatusCode)
            {
                var category = JsonConvert.DeserializeObject<GetCategoryVM>(await response.Content.ReadAsStringAsync());
                return View(category);
            }

            TempData["Error"] = "Category not found.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            // Get parent categories for dropdown
            var response = await _httpClient.GetAsync("/api/category");
            if (response.IsSuccessStatusCode)
            {
                ViewBag.ParentCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await response.Content.ReadAsStringAsync());
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AddCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                // Get parent categories again for the dropdown
                var categoriesResponse = await _httpClient.GetAsync("/api/category");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    ViewBag.ParentCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
                }

                return View(model);
            }

            // Handle file upload if present
            if (model.File != null && model.File.Length > 0)
            {
                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                // Set the image URL relative to the web root
                model.ImageUrl = "/images/categories/" + uniqueFileName;
            }

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/category", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category created successfully.";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);

            // Get parent categories again for the dropdown
            var catResponse = await _httpClient.GetAsync("/api/category");
            if (catResponse.IsSuccessStatusCode)
            {
                ViewBag.ParentCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await catResponse.Content.ReadAsStringAsync());
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"/api/category/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var category = JsonConvert.DeserializeObject<GetCategoryVM>(await response.Content.ReadAsStringAsync());
            var updateModel = UpdateCategoryVM.FromGetCategoryVM(category);

            // Get parent categories for dropdown, excluding the current category
            var categoriesResponse = await _httpClient.GetAsync("/api/category");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var allCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
                // Remove the current category and its children (if any) to prevent circular references
                ViewBag.ParentCategories = allCategories.Where(c => c.Id != id).ToList();
            }

            return View(updateModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, UpdateCategoryVM model)
        {
            if (!ModelState.IsValid)
            {
                // Get parent categories again for the dropdown
                var categoriesResponse = await _httpClient.GetAsync("/api/category");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var allCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await categoriesResponse.Content.ReadAsStringAsync());
                    ViewBag.ParentCategories = allCategories.Where(c => c.Id != id).ToList();
                }

                return View(model);
            }

            // Handle file upload if present
            if (model.File != null && model.File.Length > 0)
            {
                // Delete old image if it exists
                if (!string.IsNullOrEmpty(model.ImageUrl))
                {
                    var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, model.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images", "categories");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.File.FileName;
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                // Set the image URL relative to the web root
                model.ImageUrl = "/images/categories/" + uniqueFileName;
            }

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/category/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category updated successfully.";
                return RedirectToAction(nameof(Index));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);

            // Get parent categories again for the dropdown
            var catResponse = await _httpClient.GetAsync("/api/category");
            if (catResponse.IsSuccessStatusCode)
            {
                var allCategories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await catResponse.Content.ReadAsStringAsync());
                ViewBag.ParentCategories = allCategories.Where(c => c.Id != id).ToList();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync($"/api/category/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Category not found.";
                return RedirectToAction(nameof(Index));
            }

            var category = JsonConvert.DeserializeObject<GetCategoryVM>(await response.Content.ReadAsStringAsync());
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            SetBearerToken();
            var response = await _httpClient.DeleteAsync($"/api/category/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Category deleted successfully.";
                return RedirectToAction(nameof(Index));
            }

            TempData["Error"] = "Could not delete the category. It may have associated products.";
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> SubCategories(int parentId)
        {
            var response = await _httpClient.GetAsync($"/api/category/subcategories/{parentId}");
            if (response.IsSuccessStatusCode)
            {
                var categories = JsonConvert.DeserializeObject<List<GetCategoryVM>>(await response.Content.ReadAsStringAsync());

                // Get parent category name
                var parentResponse = await _httpClient.GetAsync($"/api/category/{parentId}");
                if (parentResponse.IsSuccessStatusCode)
                {
                    var parentCategory = JsonConvert.DeserializeObject<GetCategoryVM>(await parentResponse.Content.ReadAsStringAsync());
                    ViewBag.ParentCategoryName = parentCategory.Name;
                }

                return View("Index", categories);
            }

            TempData["Error"] = "Could not retrieve subcategories.";
            return RedirectToAction(nameof(Index));
        }
    }
}