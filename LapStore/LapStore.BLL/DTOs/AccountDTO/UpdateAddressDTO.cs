using LapStore.DAL.Data.Entities;
using System;
using System.Collections.Generic;
namespace LapStore.BLL.DTOs.AccountDTO
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
