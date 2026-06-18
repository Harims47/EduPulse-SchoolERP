using System;

namespace EduPulse.Core.Dtos.Academics
{
    public record ClassDto(
        Guid ClassId,
        string Name,
        int SortOrder
    );

    public record CreateClassRequest(
        string Name,
        int SortOrder
    );

    public record UpdateClassRequest(
        string Name,
        int SortOrder
    );
}
