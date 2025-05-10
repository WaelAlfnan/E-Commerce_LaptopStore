using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace LapStore.Web.ViewModels.AccountVM
{
    public class LoginVM
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}