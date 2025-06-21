using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.AddressDTO
{
    public class UpdateAddressDTO
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
    }
}

