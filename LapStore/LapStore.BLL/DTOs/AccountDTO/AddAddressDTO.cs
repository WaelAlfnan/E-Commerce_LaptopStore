using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class AddAddressDTO
    {
        // Address information
        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Governorate is required")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        public static Address FromAddressDTO(AddAddressDTO address)
        {
            return new Address()
            {
                Street = address.Street,
                City = address.City,
                Governorate = address.Governorate,
                Country = address.Country,
                ZipCode = address.ZipCode
            };
        }
    }
}
