using LapStore.DAL.Data.Entities;

namespace LapStore.Web.ViewModels.AccountVM
{
    public class UserInfoVM
    {
        #region Properties
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserGender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly BirthDate { get; set; }
        public int Age { get; set; }
        #endregion

    }
}