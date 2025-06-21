using Store.DAL.Data.Entities;
using System.Security.Claims;

namespace Store.BLL.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
} 
