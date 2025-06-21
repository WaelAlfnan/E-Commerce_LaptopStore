using Microsoft.AspNetCore.Identity;
using Store.DAL.Data.Entities;

namespace Store.BLL.Interfaces
{
    public interface IRoleService
    {
        Task EnsureRolesExist();
        Task<bool> RoleExistsAsync(string roleName);
        Task<bool> CreateRoleAsync(string roleName);
        Task<bool> DeleteRoleAsync(string roleName);
        Task<IEnumerable<string>> GetAllRolesAsync();
        Task<bool> UpdateRoleAsync(string currentRoleName, string newRoleName);
    }
} 
