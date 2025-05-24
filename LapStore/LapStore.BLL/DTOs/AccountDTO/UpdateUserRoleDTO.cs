using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class UpdateUserRoleDTO
    {
        [Required]
        public UserRole NewRole { get; set; }
    }
} 