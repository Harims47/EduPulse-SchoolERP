using System;

namespace EduPulse.Core.Dtos.Academics
{
    public record AcademicYearDto(
        Guid AcademicYearId,
        string Name,
        DateTime StartDate,
        DateTime EndDate
    );

    public record CreateAcademicYearRequest(
        string Name,
        DateTime StartDate,
        DateTime EndDate
    );

    public record UpdateAcademicYearRequest(
        string Name,
        DateTime StartDate,
        DateTime EndDate
    );
}
