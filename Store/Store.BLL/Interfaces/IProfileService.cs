using Store.BLL.DTOs.ProfileDTO;
using Store.BLL.Services;

namespace Store.BLL.Interfaces
{
    public interface IProfileService
    {
        
        // User profile methods
        Task<UserInfoDTO?> GetUserProfileAsync(int userId);
        Task<Result> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO);
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);

    }
}
