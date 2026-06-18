using System;

namespace EduPulse.Core.Dtos.Academics
{
    public record SectionDto(
        Guid SectionId,
        string Name
    );

    public record CreateSectionRequest(
        string Name
    );

    public record UpdateSectionRequest(
        string Name
    );
}
