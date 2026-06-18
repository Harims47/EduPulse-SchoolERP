using FluentValidation;
using EduPulse.Core.Dtos.Academics;

namespace EduPulse.Core.Validation
{
    public class CreateClassRequestValidator : AbstractValidator<CreateClassRequest>
    {
        public CreateClassRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("SortOrder cannot be negative.");
        }
    }

    public class UpdateClassRequestValidator : AbstractValidator<UpdateClassRequest>
    {
        public UpdateClassRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(30).WithMessage("Name cannot exceed 30 characters.");

            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0).WithMessage("SortOrder cannot be negative.");
        }
    }
}
