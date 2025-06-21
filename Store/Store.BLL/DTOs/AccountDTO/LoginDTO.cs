using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Store.BLL.DTOs.AccountDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Please, enter username or email")]
        public string UsernameOrEmail { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
