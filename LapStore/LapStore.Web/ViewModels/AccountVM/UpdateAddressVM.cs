namespace LapStore.Web.ViewModels.AccountVM
{
    public class UpdateAddressVM
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Governorate { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }

        public static UpdateAddressVM FromAddressInfoVM(AddressInfoVM addressInfoVM)
        {
            return new UpdateAddressVM
            {
                Street = addressInfoVM.Street,
                City = addressInfoVM.City,
                Governorate = addressInfoVM.Governorate,
                ZipCode = addressInfoVM.ZipCode,
                Country = addressInfoVM.Country
            };
        }
    }
}
