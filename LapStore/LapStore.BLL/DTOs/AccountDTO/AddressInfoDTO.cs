using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class AddressInfoDTO
    {
        public int Id { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }


        // Conversion methods
        public static AddressInfoDTO FromAddress(Address address)
        {
            return new AddressInfoDTO
            {
                Id = address.Id,
                Street = address.Street,
                City = address.City,
                Governorate = address.Governorate,
                ZipCode = address.ZipCode,
                Country = address.Country
            };
        }
    }
}