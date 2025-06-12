using LapStore.BLL.DTOs.ProfileDTO;
using LapStore.BLL.Services;

namespace LapStore.BLL.Interfaces
{
    public interface IProfileService
    {
        
        // User profile methods
        Task<UserInfoDTO?> GetUserProfileAsync(int userId);
        Task<Result> UpdateUserProfileAsync(int userId, UpdateProfileDTO updateProfileDTO);
        Task<Result> ChangePasswordAsync(int userId, ChangePasswordDTO changePasswordDTO);

    }
}