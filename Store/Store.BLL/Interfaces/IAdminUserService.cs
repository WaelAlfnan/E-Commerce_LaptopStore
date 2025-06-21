using Store.BLL.DTOs.ProfileDTO;
using Store.BLL.DTOs.AdminUserDTO;
using Store.BLL.Services;
using Store.DAL.Data.Entities;

namespace Store.BLL.Interfaces
{
    public interface IAdminUserService
    {
        
        // Admin methods
        Task<IEnumerable<UserInfoDTO>> GetAllUsersAsync();
        Task<UserInfoDTO?> GetUserByIdAsync(int userId);
        Task<Result> UpdateUserRoleAsync(int userId, int currentAdminId, UserRole newRole);
        Task<Result> DeleteUserAsync(int userId, int currentAdminId);
        Task<Result> DisableUserAsync(int userId);
        Task<Result> EnableUserAsync(int userId);

        // First Admin Registration
        Task<bool> IsFirstAdminAsync();
        Task<AuthResult> RegisterFirstAdminAsync(AdminRegisterDTO adminDTO);
    }
}
