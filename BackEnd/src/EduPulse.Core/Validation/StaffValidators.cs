using FluentValidation;
using EduPulse.Core.Dtos.Staff;

namespace EduPulse.Core.Validation
{
    public class CreateStaffRequestValidator : AbstractValidator<CreateStaffRequest>
    {
        public CreateStaffRequestValidator()
        {
            RuleFor(x => x.EmployeeCode)
                .NotEmpty().WithMessage("EmployeeCode is required.")
                .MaximumLength(50).WithMessage("EmployeeCode cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(100).WithMessage("FirstName cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(100).WithMessage("LastName cannot exceed 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(15).WithMessage("Phone cannot exceed 15 characters.");

            RuleFor(x => x.Designation)
                .MaximumLength(50).WithMessage("Designation cannot exceed 50 characters.");

            RuleFor(x => x.PhotoPath)
                .MaximumLength(500).WithMessage("PhotoPath cannot exceed 500 characters.");
        }
    }

    public class UpdateStaffRequestValidator : AbstractValidator<UpdateStaffRequest>
    {
        public UpdateStaffRequestValidator()
        {
            RuleFor(x => x.EmployeeCode)
                .NotEmpty().WithMessage("EmployeeCode is required.")
                .MaximumLength(50).WithMessage("EmployeeCode cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("FirstName is required.")
                .MaximumLength(100).WithMessage("FirstName cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("LastName is required.")
                .MaximumLength(100).WithMessage("LastName cannot exceed 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .MaximumLength(15).WithMessage("Phone cannot exceed 15 characters.");

            RuleFor(x => x.Designation)
                .MaximumLength(50).WithMessage("Designation cannot exceed 50 characters.");

            RuleFor(x => x.PhotoPath)
                .MaximumLength(500).WithMessage("PhotoPath cannot exceed 500 characters.");
        }
    }
}
