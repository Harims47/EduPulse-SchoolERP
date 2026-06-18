using System;

namespace EduPulse.Core.Domain.Entities
{
    public class Student
    {
        public Guid StudentId { get; set; }
        public Guid TenantId { get; set; }
        public string AdmissionNo { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? BloodGroup { get; set; }
        public string? GovernmentIdType { get; set; }
        public string? GovernmentIdNumber { get; set; }
        public string? SocialCategory { get; set; }
        public string? PhotoPath { get; set; }
        public string? AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Pincode { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Status { get; set; } = "Applied";
        public Guid ClassId { get; set; }
        public Guid SectionId { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
