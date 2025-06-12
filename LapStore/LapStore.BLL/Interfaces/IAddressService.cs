using LapStore.BLL.DTOs.AddressDTO;
using LapStore.BLL.Services;

namespace LapStore.BLL.Interfaces
{
    public interface IAddressService
    {

        // Address methods
        Task<AddressInfoDTO?> GetUserAddressAsync(int userId);
        Task<Result> AddAddressAsync(int userId, AddAddressDTO addressDTO);
        Task<Result> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO);

    }
}