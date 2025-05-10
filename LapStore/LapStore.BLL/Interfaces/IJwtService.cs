using LapStore.BLL.DTOs.CartDTOs;
using LapStore.DAL.Data.Entities;
using System.Security.Claims;

namespace LapStore.BLL.Interfaces
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        ClaimsPrincipal? ValidateToken(string token);
    }
} 