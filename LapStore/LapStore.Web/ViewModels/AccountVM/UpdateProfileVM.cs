using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LapStore.Web.ViewModels.AccountVM
{
    // Update User Profile
    public class UpdateProfileVM
    {
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string UserName { get; set; }


        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }


        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }


        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        public UserGender Gender { get; set; }

        [DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; }

        public string PhoneNumber { get; set; }
    }
}
