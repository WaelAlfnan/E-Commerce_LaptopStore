using Store.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Store.BLL.DTOs.AdminUserDTO
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public UserRole NewRole { get; set; }
    }
} 
