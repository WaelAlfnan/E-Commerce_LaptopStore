using LapStore.DAL.Data.Entities;
using Microsoft.AspNetCore.Identity;
using LapStore.BLL.Interfaces;
using LapStore.BLL.DTOs.AccountDTO;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace LapStore.BLL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AccountService> _logger;
        private readonly IRoleService _roleService;
        private readonly IUserClaimService _userClaimService;

        public AccountService(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ILogger<AccountService> logger,
            IRoleService roleService,
            IUserClaimService userClaimService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtService = jwtService;
            _logger = logger;
            _roleService = roleService;
            _userClaimService = userClaimService;
        }

        public async Task<AuthResult> Register(RegisterDTO registerDTO)
        {
            try
            {
                var user = RegisterDTO.FromRegisterDTO(registerDTO);

                var result = await _userManager.CreateAsync(user, registerDTO.Password);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Registration failed for user {Username}: {Errors}", 
                        registerDTO.UserName, string.Join(", ", errors));
                    return new AuthResult { Success = false, Errors = errors };
                }

                // Ensure Customer role exists
                if (!await _roleService.RoleExistsAsync(UserRole.Customer.ToString()))
                {
                    await _roleService.CreateRoleAsync(UserRole.Customer.ToString());
                }

                // Add customer role
                await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());

                // Add registration claims
                var claimResult = await _userClaimService.AddRegistrationClaimsAsync(user);
                if (!claimResult.Success)
                {
                    _logger.LogWarning("Failed to add registration claims for user {Username}: {Error}", 
                        registerDTO.UserName, claimResult.Message);
                }

                // Add role claim
                var roleClaimResult = await _userClaimService.AddClaimAsync(user.Id, ClaimTypes.Role, UserRole.Customer.ToString());
                if (!roleClaimResult.Success)
                {
                    _logger.LogWarning("Failed to add role claim for user {Username}: {Error}", 
                        registerDTO.UserName, roleClaimResult.Message);
                }

                var token = await _jwtService.GenerateTokenAsync(user);
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration for {Username}", registerDTO.UserName);
                return new AuthResult { Success = false, Errors = new[] { "An unexpected error occurred during registration." } };
            }
        }

        public async Task<AuthResult> Login(LoginDTO loginDTO)
        {
            try
            {
                // Try to find user by username first
                var user = await _userManager.FindByNameAsync(loginDTO.UsernameOrEmail);
                // If not found, try to find by email
                if (user == null && !string.IsNullOrWhiteSpace(loginDTO.UsernameOrEmail))
                {
                    user = await _userManager.FindByEmailAsync(loginDTO.UsernameOrEmail);
                }
                if (user == null)
                {
                    _logger.LogWarning("Login attempt failed: User {UsernameOrEmail} not found", loginDTO.UsernameOrEmail);
                    return new AuthResult { Success = false, Errors = new[] { "Invalid username/email or password." } };
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginDTO.Password, false);
                if (!result.Succeeded)
                {
                    _logger.LogWarning("Login attempt failed for user {UsernameOrEmail}: Invalid password", loginDTO.UsernameOrEmail);
                    return new AuthResult { Success = false, Errors = new[] { "Invalid username/email or password." } };
                }

                var token = await _jwtService.GenerateTokenAsync(user);
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login for user {UsernameOrEmail}", loginDTO.UsernameOrEmail);
                return new AuthResult { Success = false, Errors = new[] { "An unexpected error occurred during login." } };
            }
        }

        public async Task<bool> LogoutAsync(string username)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(username);
                if (user == null)
                {
                    _logger.LogWarning("Logout attempt failed: User {Username} not found", username);
                    return false;
                }

                await _signInManager.SignOutAsync();
                _logger.LogInformation("User {Username} logged out successfully", username);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during logout for user {Username}", username);
                return false;
            }
        }

        public async Task<AuthenticationProperties> GetGoogleAuthPropertiesAsync(string provider, string returnUrl)
        {
            try
            {
                var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, returnUrl);
                return properties;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error configuring Google authentication properties");
                throw;
            }
        }

        public async Task<AuthResult> HandleGoogleCallbackAsync()
        {
            try
            {
                // السطر ده بيحاول يجيب معلومات تسجيل الدخول الخارجي (من جوجل) اللي جوجل رجعها لنا.
                var info = await _signInManager.GetExternalLoginInfoAsync();
                // لو الـ info دي فاضية، يبقى فيه مشكلة، جوجل ما رجعش بيانات.
                if (info == null)
                {
                    // بنسجل تحذير في الـ logs إننا مقدرناش نجيب البيانات.
                    _logger.LogWarning("Failed to get external login information from Google");
                    // وبنرجع نتيجة إن العملية فشلت، وبنقول السبب.
                    return new AuthResult
                    {
                        Success = false,
                        Errors = new[] { "Error loading external login information." }
                    };
                }

                // --- هنا بنحاول ندخل اليوزر لو كان مسجل عندنا قبل كده ---

                // السطر ده بيحاول يدخل اليوزر باستخدام بيانات تسجيل الدخول اللي جاية من جوجل.
                // الـ "info.LoginProvider" هو "Google"، والـ "info.ProviderKey" ده الـ ID بتاع اليوزر عند جوجل.
                // الـ "isPersistent: false" يعني اليوزر مش هيفضل داخل حتى لو قفل المتصفح.
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                // لو الـ login نجح (يعني اليوزر ده مسجل عندنا قبل كده بنفس حساب جوجل)،
                if (result.Succeeded)
                {
                    // بنجيب بيانات اليوزر بتاعنا في الداتابيز.
                    var userInfo = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    // بنولد له JWT token عشان يعرف يكمل شغل في الموقع.
                    var jwtToken = await _jwtService.GenerateTokenAsync(userInfo);
                    // وبنرجع نتيجة إن العملية نجحت ومعاها الـ token.
                    return new AuthResult { Success = true, Token = jwtToken };
                }

                // --- لو اليوزر مكنش مسجل عندنا، يبقى لازم نعمله حساب جديد ---

                // بنجيب الإيميل بتاع اليوزر من البيانات اللي جات من جوجل.
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                // بنجيب اسم المستخدم بتاع اليوزر من البيانات اللي جات من جوجل.
                var userName = info.Principal.FindFirstValue(ClaimTypes.Name);
                // بنشوف لو فيه يوزر عندنا بالإيميل ده.
                var user = await _userManager.FindByEmailAsync(email);
                // لو مفيش يوزر بالإيميل ده خالص، يبقى ده يوزر جديد لسه بيسجل لأول مرة.
                if (user == null)
                {
                    // بنعمل أوبجكت User جديد ببياناته اللي جبناها من جوجل.
                    // الـ EmailConfirmed بنحطها true عشان جوجل أكد الإيميل خلاص.
                    // وبنجيب الاسم الأول والأخير لو موجودين.
                    user = new User
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true, // Google already confirmed
                        FirstName = info.Principal.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty,
                        LastName = info.Principal.FindFirstValue(ClaimTypes.Surname) ?? string.Empty
                    };

                    // بنحاول نكرييت اليوزر ده في الداتابيز بتاعتنا.
                    var createResult = await _userManager.CreateAsync(user);
                    // لو عملية الـ creation فشلت،
                    if (!createResult.Succeeded)
                    {
                        // بنجيب الأخطاء اللي حصلت.
                        var errors = createResult.Errors.Select(e => e.Description);
                        // بنسجل تحذير في الـ logs إننا مقدرناش نكريت اليوزر.
                        _logger.LogWarning("Failed to create user from Google login: {Errors}",
                            string.Join(", ", errors));
                        // وبنرجع إن العملية فشلت ومعاها الأخطاء.
                        return new AuthResult { Success = false, Errors = errors };
                    }

                    // --- هنا بنضمن إن الـ role بتاع Customer موجود وبنضيفه لليوزر الجديد ---

                    // بنشوف لو الـ role اللي اسمه "Customer" موجود عندنا في الداتابيز.
                    if (!await _roleService.RoleExistsAsync(UserRole.Customer.ToString()))
                    {
                        // لو مش موجود، بنعمله.
                        await _roleService.CreateRoleAsync(UserRole.Customer.ToString());
                    }

                    // بنضيف اليوزر الجديد للـ role بتاع "Customer".
                    await _userManager.AddToRoleAsync(user, UserRole.Customer.ToString());

                    // --- هنا بنضيف Claims إضافية لليوزر الجديد ---

                    // بنضيف claims معينة لليوزر ده عشان نعرف إنه سجل عن طريق جوجل.
                    var claimResult = await _userClaimService.AddRegistrationClaimsAsync(user, "Google");
                    // لو عملية إضافة الـ claims دي فشلت،
                    if (!claimResult.Success)
                    {
                        // بنسجل تحذير في الـ logs.
                        _logger.LogWarning("Failed to add registration claims for Google user {UserId}: {Error}",
                            user.Id, claimResult.Message);
                    }
                }

                // --- هنا بنربط حساب جوجل باليوزر بتاعنا (سواء كان جديد أو قديم) ---

                // بنربط حساب جوجل (External Login) باليوزر بتاعنا في الداتابيز.
                var addLoginResult = await _userManager.AddLoginAsync(user, info);
                // لو عملية الربط فشلت،
                if (!addLoginResult.Succeeded)
                {
                    // بنجيب الأخطاء وبنسجل تحذير.
                    var errors = addLoginResult.Errors.Select(e => e.Description);
                    _logger.LogWarning("Failed to link Google account for user {UserId}: {Errors}",
                        user.Id, string.Join(", ", errors));
                    // وبنرجع إن العملية فشلت.
                    return new AuthResult { Success = false, Errors = errors };
                }

                // --- أخيراً، بندخل اليوزر وبنولد له Token ---

                // بنعمل sign in لليوزر (المرة دي لليوزر بتاعنا في السيستم).
                await _signInManager.SignInAsync(user, isPersistent: false);
                // بنولد له JWT token عشان يعرف يكمل شغل في الموقع.
                var token = await _jwtService.GenerateTokenAsync(user);
                // وبنرجع نتيجة إن العملية نجحت ومعاها الـ token.
                return new AuthResult { Success = true, Token = token };
            }
            catch (Exception ex)
            {
                // لو حصل أي مشكلة غير متوقعة في أي حتة في الميثود دي،
                // بنسجل المشكلة في الـ logs.
                _logger.LogError(ex, "Error handling Google callback");
                // وبنرجع نتيجة إن العملية فشلت ومعاها رسالة خطأ عامة.
                return new AuthResult
                {
                    Success = false,
                    Errors = new[] { "An unexpected error occurred during Google authentication." }
                };
            }
        }
    }
}