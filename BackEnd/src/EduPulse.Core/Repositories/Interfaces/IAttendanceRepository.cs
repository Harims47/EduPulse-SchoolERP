using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Attendance;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<AttendanceSession?> GetSessionAsync(Guid tenantId, Guid classId, Guid sectionId, DateTime date);
        Task<Guid> CreateSessionAsync(AttendanceSession session);
        Task<IEnumerable<AttendanceEntryDto>> GetEntriesBySessionIdAsync(Guid tenantId, Guid sessionId);
        Task<bool> UpsertEntriesAsync(Guid tenantId, Guid sessionId, List<AttendanceEntry> entries);
        Task<IEnumerable<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(Guid tenantId, Guid studentId);
        Task<IEnumerable<ClassAttendanceSummaryDto>> GetClassSummaryAsync(Guid tenantId, Guid classId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
