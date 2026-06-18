using System;

namespace EduPulse.Core.Dtos.Guardians
{
    public record GuardianDto(
        Guid GuardianId,
        Guid? UserId,
        string FirstName,
        string LastName,
        string Phone,
        string? Email
    );

    public record CreateGuardianRequest(
        Guid? UserId,
        string FirstName,
        string LastName,
        string Phone,
        string? Email
    );

    public record UpdateGuardianRequest(
        Guid? UserId,
        string FirstName,
        string LastName,
        string Phone,
        string? Email
    );
}
