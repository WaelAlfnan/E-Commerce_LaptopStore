using Store.DAL.Data.Entities;
using Store.DAL.Repositories;
using Store.DAL;
using Microsoft.AspNetCore.Identity;
using Store.BLL.Interfaces;
using Store.BLL.DTOs.AddressDTO;
using Microsoft.Extensions.Logging;

namespace Store.BLL.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Address> _addressRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<AccountService> _logger;

        public AddressService(
            IUnitOfWork unitOfWork,
            IGenericRepository<Address> addressRepository,
            IUserRepository userRepository,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IJwtService jwtService,
            ILogger<AccountService> logger,
            IRoleService roleService,
            IUserClaimService userClaimService)
        {
            _unitOfWork = unitOfWork;
            _addressRepository = addressRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _logger = logger;
        }

        
        public async Task<AddressInfoDTO?> GetUserAddressAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                if (user?.address == null)
                {
                    return null;
                }

                return AddressInfoDTO.FromAddress(user.address);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving address for user {UserId}", userId);
                return null;
            }
        }

        public async Task<Result> AddAddressAsync(int userId, AddAddressDTO addressDTO)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    return new Result { Success = false, Message = "User not found." };
                }

                var address = AddAddressDTO.FromAddressDTO(addressDTO);
                await _addressRepository.AddAsync(address);
                await _unitOfWork.CompleteAsync();

                user.AddressId = address.Id;
                var result = await _userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    var errors = result.Errors.Select(e => e.Description);
                    _logger.LogWarning("Address update failed for user {UserId}: {Errors}", 
                        userId, string.Join(", ", errors));
                    return new Result { Success = false, Message = string.Join(", ", errors) };
                }

                return new Result { Success = true, Message = "Address added successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding address for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

        public async Task<Result> UpdateAddressAsync(int userId, int addressId, UpdateAddressDTO addressDTO)
        {
            try
            {
                var user = await _userRepository.GetUserWithAddressAsync(userId);
                if (user?.address == null || user.address.Id != addressId)
                {
                    return new Result { Success = false, Message = "Address not found." };
                }

                user.address.Street = addressDTO.Street;
                user.address.City = addressDTO.City;
                user.address.Governorate = addressDTO.Governorate;
                user.address.Country = addressDTO.Country;
                user.address.ZipCode = addressDTO.ZipCode;
                await _unitOfWork.CompleteAsync();

                return new Result { Success = true, Message = "Address updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating address for user {UserId}", userId);
                return new Result { Success = false, Message = "An unexpected error occurred." };
            }
        }

    }
}
