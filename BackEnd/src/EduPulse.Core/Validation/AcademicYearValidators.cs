using FluentValidation;
using EduPulse.Core.Dtos.Academics;

namespace EduPulse.Core.Validation
{
    public class CreateAcademicYearRequestValidator : AbstractValidator<CreateAcademicYearRequest>
    {
        public CreateAcademicYearRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        }
    }

    public class UpdateAcademicYearRequestValidator : AbstractValidator<UpdateAcademicYearRequest>
    {
        public UpdateAcademicYearRequestValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(20).WithMessage("Name cannot exceed 20 characters.");

            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("StartDate is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("EndDate is required.")
                .GreaterThan(x => x.StartDate).WithMessage("EndDate must be after StartDate.");
        }
    }
}
