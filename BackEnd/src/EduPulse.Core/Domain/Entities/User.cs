using System;
using System.Collections.Generic;

namespace EduPulse.Core.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public Guid TenantId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class UserWithRoles
    {
        public User User { get; set; } = null!;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
