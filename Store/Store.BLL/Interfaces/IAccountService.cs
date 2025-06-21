using Store.BLL.DTOs.AccountDTO;
using Store.BLL.Services;
using Microsoft.AspNetCore.Authentication;

namespace Store.BLL.Interfaces
{
    public interface IAccountService
    {
        // Authentication methods
        Task<AuthResult> Register(RegisterDTO registerDTO);
        Task<AuthResult> Login(LoginDTO loginDTO);
        Task<bool> LogoutAsync(string username);

        // Google Authentication methods
        Task<AuthenticationProperties> GetGoogleAuthPropertiesAsync(string provider, string returnUrl);
        Task<AuthResult> HandleGoogleCallbackAsync();
    }
}
