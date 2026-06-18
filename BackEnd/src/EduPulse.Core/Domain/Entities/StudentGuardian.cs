using System;

namespace EduPulse.Core.Domain.Entities
{
    public class StudentGuardian
    {
        public Guid StudentId { get; set; }
        public Guid GuardianId { get; set; }
        public Guid TenantId { get; set; }
        public string RelationshipType { get; set; } = string.Empty;
        public bool IsPrimaryContact { get; set; }
        public bool IsBillingResponsible { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
