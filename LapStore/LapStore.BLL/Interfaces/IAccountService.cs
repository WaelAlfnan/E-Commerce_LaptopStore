using LapStore.BLL.DTOs.AccountDTO;
using LapStore.BLL.Services;
using Microsoft.AspNetCore.Authentication;

namespace LapStore.BLL.Interfaces
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