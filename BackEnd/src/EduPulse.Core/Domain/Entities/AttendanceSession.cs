using System;

namespace EduPulse.Core.Domain.Entities
{
    public class AttendanceSession
    {
        public Guid AttendanceSessionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid ClassId { get; set; }
        public Guid SectionId { get; set; }
        public DateTime Date { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
