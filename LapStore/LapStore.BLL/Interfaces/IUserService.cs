using LapStore.BLL.DTOs.AccountDTO;
using LapStore.BLL.Services;
using LapStore.DAL.Data.Entities;

namespace LapStore.BLL.Interfaces
{
    public interface IUserService
    {
        // Authentication methods
        Task<AuthResult> Register(RegisterDTO registerDTO);
        Task<AuthResult> Login(LoginDTO loginDTO);
        Task<bool> LogoutAsync(string username);

        // User profile methods
        Task<UserInfoDTO?> GetUserProfileAsync(int userId);
        Task<Result> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO);
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);

        // Address methods
        Task<AddressInfoDTO?> GetUserAddressAsync(int userId);
        Task<Result> AddAddressAsync(int userId, AddAddressDTO addressDTO);
        Task<Result> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO);

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