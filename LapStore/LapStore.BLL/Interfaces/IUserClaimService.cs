using LapStore.BLL.Services;
using LapStore.DAL.Data.Entities;
using System.Security.Claims;

namespace LapStore.BLL.Interfaces
{
    public interface IUserClaimService
    {
        // Basic claim operations
        Task<Result> AddClaimAsync(int userId, string claimType, string claimValue);
        Task<Result> AddClaimsAsync(int userId, IEnumerable<Claim> claims);
        Task<Result> UpdateClaimAsync(int userId, string claimType, string oldValue, string newValue);
        Task<Result> RemoveClaimAsync(int userId, string claimType, string claimValue);
        Task<IEnumerable<Claim>> GetUserClaimsAsync(int userId);
        Task<bool> HasClaimAsync(int userId, string claimType, string claimValue);

        // Profile-specific claim operations
        Task<Result> UpdateProfileClaimsAsync(int userId, string userName, string email);
        Task<Result> UpdateRoleClaimsAsync(int userId, string role);
        
        // Registration-specific claim operations
        Task<Result> AddRegistrationClaimsAsync(User user, string registrationSource = "Web");
        Task<Result> AddAdminRegistrationClaimsAsync(User user);
    }
} 