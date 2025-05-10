using LapStore.DAL.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class UpdateAddressDTO
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }



        public static Address FromAddressDTO(UpdateAddressDTO addressDTO)
        {
            return new Address
            {
                Street = addressDTO.Street,
                City = addressDTO.City,
                Governorate = addressDTO.Governorate,
                ZipCode = addressDTO.ZipCode,
                Country = addressDTO.Country
            };
        }
    }
}
