using System.ComponentModel.DataAnnotations;

namespace LapStore.Web.ViewModels.AccountVM
{
    public class AddAddressVM
    {
        [Required(ErrorMessage = "Street is required")]
        public string Street { get; set; }

        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [Required(ErrorMessage = "Governorate is required")]
        public string Governorate { get; set; }

        [Required(ErrorMessage = "Zip code is required")]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; }
    }

}
