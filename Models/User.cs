using System;

namespace InitialSetupMVC.Models
{
    public class User
    {
        public long Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public long RoleId { get; set; }
        public string RoleName { get; set; } = string.Empty; // "Admin", "User Distribution", "External User"
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
