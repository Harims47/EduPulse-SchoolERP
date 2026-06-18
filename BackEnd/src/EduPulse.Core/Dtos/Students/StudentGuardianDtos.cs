using System;

namespace EduPulse.Core.Dtos.Students
{
    public record StudentGuardianDto(
        Guid StudentId,
        Guid GuardianId,
        string RelationshipType,
        bool IsPrimaryContact,
        bool IsBillingResponsible,
        string FirstName,
        string LastName,
        string Phone
    );

    public record LinkGuardianRequest(
        Guid GuardianId,
        string RelationshipType,
        bool IsPrimaryContact,
        bool IsBillingResponsible
    );

    public record UpdateRelationshipRequest(
        string RelationshipType,
        bool IsPrimaryContact,
        bool IsBillingResponsible
    );
}
