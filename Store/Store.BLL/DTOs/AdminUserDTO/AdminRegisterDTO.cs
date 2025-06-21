using Store.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.AdminUserDTO
{
    public class AdminRegisterDTO
    {
        [Required(ErrorMessage = "Username is required")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{6,}$",
            ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Birth date is required")]
        public DateOnly BirthDate { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        public UserGender Gender { get; set; }


        public static User FromAdminRegisterDTO(AdminRegisterDTO adminDTO)
        {
            return new User
            {
                UserName = adminDTO.UserName,
                Email = adminDTO.Email,
                FirstName = adminDTO.FirstName,
                LastName = adminDTO.LastName,
                PhoneNumber = adminDTO.PhoneNumber,
                BirthDate = adminDTO.BirthDate,
                Gender = adminDTO.Gender,
                Role = UserRole.Admin,
                EmailConfirmed = true // Auto-confirm email for first admin
            };
        }
    }
} 
