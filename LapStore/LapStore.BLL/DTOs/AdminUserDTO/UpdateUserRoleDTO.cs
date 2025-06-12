using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.AdminUserDTO
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public UserRole NewRole { get; set; }
    }
} 