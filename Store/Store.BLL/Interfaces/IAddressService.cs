using Store.BLL.DTOs.AddressDTO;
using Store.BLL.Services;

namespace Store.BLL.Interfaces
{
    public interface IAddressService
    {

        // Address methods
        Task<AddressInfoDTO?> GetUserAddressAsync(int userId);
        Task<Result> AddAddressAsync(int userId, AddAddressDTO addressDTO);
        Task<Result> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO);

    }
}
