using System;

namespace EduPulse.Core.Domain.Entities
{
    public class Staff
    {
        public Guid StaffId { get; set; }
        public Guid TenantId { get; set; }
        public Guid? UserId { get; set; }
        public string EmployeeCode { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string? PhotoPath { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
