using LapStore.DAL.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace LapStore.BLL.DTOs.AccountDTO
{
    public class UserManagementDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
} 