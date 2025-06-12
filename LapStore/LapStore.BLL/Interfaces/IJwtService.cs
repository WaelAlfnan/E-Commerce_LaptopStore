using LapStore.DAL.Data.Entities;
using System.Security.Claims;

namespace LapStore.BLL.Interfaces
{
    public interface IJwtService
    {
        Task<string> GenerateTokenAsync(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
} 