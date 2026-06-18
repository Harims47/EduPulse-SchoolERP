using System;

namespace EduPulse.Core.Domain.Entities
{
    public class AcademicYear
    {
        public Guid AcademicYearId { get; set; }
        public Guid TenantId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
