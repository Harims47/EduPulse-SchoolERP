using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Attendance;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<bool> MarkAttendanceAsync(MarkAttendanceRequest request);
        Task<DailyAttendanceReport?> GetDailyAttendanceAsync(Guid classId, Guid sectionId, DateTime date);
        Task<IEnumerable<StudentAttendanceHistoryDto>> GetStudentHistoryAsync(Guid studentId);
        Task<IEnumerable<ClassAttendanceSummaryDto>> GetClassSummaryAsync(Guid classId, DateTime? startDate = null, DateTime? endDate = null);
    }
}
