using System;
using System.Collections.Generic;

namespace EduPulse.Core.Dtos.Attendance
{
    public record AttendanceEntryRequest(
        Guid StudentId,
        string Status,
        string? Remarks
    );

    public record MarkAttendanceRequest(
        Guid ClassId,
        Guid SectionId,
        DateTime Date,
        List<AttendanceEntryRequest> Entries
    );

    public record AttendanceEntryDto(
        Guid StudentId,
        string FirstName,
        string LastName,
        string AdmissionNo,
        string Status,
        string? Remarks
    );

    public record DailyAttendanceReport(
        Guid AttendanceSessionId,
        Guid ClassId,
        Guid SectionId,
        DateTime Date,
        List<AttendanceEntryDto> Entries
    );

    public record StudentAttendanceHistoryDto(
        DateTime Date,
        Guid ClassId,
        string ClassName,
        Guid SectionId,
        string SectionName,
        string Status,
        string? Remarks
    );

    public record ClassAttendanceSummaryDto(
        DateTime Date,
        int TotalPresent,
        int TotalAbsent,
        int TotalLate,
        int TotalHalfDay
    );
}
