using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace LapStore.DAL.Data.Entities
{
    public class User : IdentityUser<int>  // Inherit from IdentityUser with int as key type
    {
        [Required(ErrorMessage = "Please select your gender")]
        public UserGender Gender { get; set; }

        [Required(ErrorMessage = "Please enter your first name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your last name")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter your birth date")]
        public DateOnly BirthDate
        {
            get { return _birthDate; }
            set
            {
                DateOnly minDate = new DateOnly(1900, 1, 1);
                DateOnly maxDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5));

                if (value < minDate || value > maxDate)
                {
                    throw new ArgumentOutOfRangeException(nameof(BirthDate), $"The birth date must be between 1900 and {maxDate.Year}");
                }

                _birthDate = value;
            }
        }
        [NotMapped]
        private DateOnly _birthDate;

        public int Age
        {
            get
            {
                DateTime currentDate = DateTime.Today;
                int age = currentDate.Year - BirthDate.Year;

                if (BirthDate.Month > currentDate.Month || BirthDate.Month == currentDate.Month && BirthDate.Day > currentDate.Day)
                {
                    age--;
                }

                return age;
            }
        }

        public UserRole Role { get; set; } = UserRole.Customer;

        // Modified to be nullable to allow registration without an address initially
        [ForeignKey("address")]
        public int? AddressId { get; set; }

        // Navigation Properties
        public virtual ICollection<Order>? orders { get; set; }
        public virtual Cart? cart { get; set; }
        public virtual Address? address { get; set; }
        public virtual ICollection<Review>? userReviews { get; set; }
    }
}