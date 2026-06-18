using System;

namespace EduPulse.Core.Domain.Entities
{
    public class AttendanceEntry
    {
        public Guid AttendanceEntryId { get; set; }
        public Guid AttendanceSessionId { get; set; }
        public Guid StudentId { get; set; }
        public Guid TenantId { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Remarks { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedOn { get; set; }
        public Guid CreatedByUserId { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public Guid? ModifiedByUserId { get; set; }
    }
}
