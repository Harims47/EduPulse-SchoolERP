using FluentValidation;
using EduPulse.Core.Dtos.Students;

namespace EduPulse.Core.Validation
{
    public class LinkGuardianRequestValidator : AbstractValidator<LinkGuardianRequest>
    {
        public LinkGuardianRequestValidator()
        {
            RuleFor(x => x.GuardianId)
                .NotEmpty().WithMessage("Guardian ID is required.");

            RuleFor(x => x.RelationshipType)
                .NotEmpty().WithMessage("Relationship type is required.")
                .Must(r => r == "Father" || r == "Mother" || r == "Guardian" || r == "Other")
                .WithMessage("Invalid Relationship Type. Must be 'Father', 'Mother', 'Guardian', or 'Other'.");
        }
    }

    public class UpdateRelationshipRequestValidator : AbstractValidator<UpdateRelationshipRequest>
    {
        public UpdateRelationshipRequestValidator()
        {
            RuleFor(x => x.RelationshipType)
                .NotEmpty().WithMessage("Relationship type is required.")
                .Must(r => r == "Father" || r == "Mother" || r == "Guardian" || r == "Other")
                .WithMessage("Invalid Relationship Type. Must be 'Father', 'Mother', 'Guardian', or 'Other'.");
        }
    }
}
