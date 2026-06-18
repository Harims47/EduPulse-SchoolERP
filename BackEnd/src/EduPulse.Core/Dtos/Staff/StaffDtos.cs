using System;

namespace EduPulse.Core.Dtos.Staff
{
    public record StaffDto(
        Guid StaffId,
        Guid? UserId,
        string EmployeeCode,
        string FirstName,
        string LastName,
        string Phone,
        string? Designation,
        string? PhotoPath,
        bool IsActive
    );

    public record CreateStaffRequest(
        Guid? UserId,
        string EmployeeCode,
        string FirstName,
        string LastName,
        string Phone,
        string? Designation,
        string? PhotoPath,
        bool IsActive
    );

    public record UpdateStaffRequest(
        Guid? UserId,
        string EmployeeCode,
        string FirstName,
        string LastName,
        string Phone,
        string? Designation,
        string? PhotoPath,
        bool IsActive
    );
}
