using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using LapStore.Web.ViewModels.AccountVM;
using Newtonsoft.Json;
using System.Text;
using System.Net.Http.Json;
using System.IdentityModel.Tokens.Jwt;

namespace LapStore.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClient = httpClientFactory.CreateClient("ApiClient");
            _configuration = configuration;
        }


        private void SetBearerToken()
        {
            try
            {
                var token = HttpContext.Session.GetString("Token");
                if (string.IsNullOrEmpty(token))
                {
                    Console.WriteLine("Warning: No token found in session");
                    Console.WriteLine($"Session ID: {HttpContext.Session.Id}");
                    Console.WriteLine($"Is Session Available: {HttpContext.Session.IsAvailable}");
                    return;
                }
                
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                Console.WriteLine("Token set successfully in request headers");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error setting bearer token: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginVM model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _httpClient.PostAsJsonAsync<LoginVM>("/api/account/Login", model);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();

                // Define a simple class for the token-only response
                var tokenResponse = JsonConvert.DeserializeObject<TokenOnlyResponse>(responseString);

                if (string.IsNullOrEmpty(tokenResponse.Token))
                {
                    ModelState.AddModelError("", "Server returned invalid token data. Please try again.");
                    return View(model);
                }

                // Decode JWT token to extract user information
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(tokenResponse.Token);

                // Extract claims from token
                var userName = jwtToken.Claims.FirstOrDefault(c => c.Type == "UserName")?.Value;
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier")?.Value;
                var role = jwtToken.Claims.FirstOrDefault(c => c.Type == "Role")?.Value;

                // Check if required user data was extracted
                if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError("", "Failed to extract user data from token. Please try again.");
                    return View(model);
                }

                // Store token in session
                HttpContext.Session.SetString("Token", tokenResponse.Token);

                // Create claims identity with extracted data
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName),
                    new Claim(ClaimTypes.NameIdentifier, userId),
                    new Claim(ClaimTypes.Role, role ?? "User") // Provide a default role if null
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = model.RememberMe };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                return !string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl)
                    ? Redirect(returnUrl)
                    : RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var response = await _httpClient.PostAsJsonAsync<RegisterVM>("/api/account/Register", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Registration successful! Please check your email.";
                return RedirectToAction(nameof(Login));
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Remove("Token");
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/Profile");
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Profile request failed with status code: {response.StatusCode}");
                Console.WriteLine($"Error content: {errorContent}");
                return RedirectToAction("Login");
            }

            var content = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrEmpty(content))
            {
                Console.WriteLine("Warning: Profile endpoint returned empty content");
                return RedirectToAction("Login");
            }

            try
            {
                var user = JsonConvert.DeserializeObject<UserInfoVM>(content);
                if (user == null)
                {
                    Console.WriteLine("Warning: Failed to deserialize user profile data");
                    return RedirectToAction("Login");
                }
                return View(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deserializing profile data: {ex.Message}");
                return RedirectToAction("Login");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditProfile()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/profile");
            if (!response.IsSuccessStatusCode) return RedirectToAction("Login");

            var user = JsonConvert.DeserializeObject<UpdateProfileVM>(await response.Content.ReadAsStringAsync());
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(UpdateProfileVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var response = await _httpClient.PutAsJsonAsync<UpdateProfileVM>("/api/account/Profile", model);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Profile updated.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Could not update profile.");
            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/change-password", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Password changed.";
                return RedirectToAction("Profile");
            }

            ModelState.AddModelError("", "Password change failed.");
            return View(model);
        }
        [HttpGet("AddressInfo")]
        public async Task<IActionResult> AddressInfo()
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/address");

            if (response.IsSuccessStatusCode)
            {
                var address = JsonConvert.DeserializeObject<AddressInfoVM>(await response.Content.ReadAsStringAsync());
                return View(address);
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                // If no address found, redirect to add address page
                return RedirectToAction(nameof(AddAddress));
            }

            TempData["Error"] = "Could not retrieve address information.";
            return RedirectToAction("Profile");
        }


        [HttpGet("AddAddress")]
        public IActionResult AddAddress()
        {
            return View();
        }

        [HttpPost("AddAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress(AddAddressVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/account/address", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address added successfully.";
                return RedirectToAction("AddressInfo");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }

        [HttpGet("EditAddress")]
        public async Task<IActionResult> EditAddress(int id)
        {
            SetBearerToken();
            var response = await _httpClient.GetAsync("/api/account/address");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Could not retrieve address information.";
                return RedirectToAction("Profile");
            }

            var address = JsonConvert.DeserializeObject<AddressInfoVM>(await response.Content.ReadAsStringAsync());

            if (address.Id != id)
            {
                TempData["Error"] = "Address not found.";
                return RedirectToAction("Address");
            }

            var updateModel = UpdateAddressVM.FromAddressInfoVM(address);

            return View(updateModel);
        }

        [HttpPost("EditAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAddress(int id, UpdateAddressVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            SetBearerToken();
            var content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"/api/account/address/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Address updated successfully.";
                return RedirectToAction("AddressInfo");
            }

            var error = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError("", error);
            return View(model);
        }
    }
}