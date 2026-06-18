using FluentValidation;
using EduPulse.Core.Dtos.Students;
using System;

namespace EduPulse.Core.Validation
{
    public class CreateStudentRequestValidator : AbstractValidator<CreateStudentRequest>
    {
        public CreateStudentRequestValidator()
        {
            RuleFor(x => x.AdmissionNo)
                .NotEmpty().WithMessage("Admission number is required.")
                .MaximumLength(50).WithMessage("Admission number cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .MaximumLength(10).WithMessage("Gender cannot exceed 10 characters.");

            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("Admission date is required.");

            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("Class Standard ID is required.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "Applied" || s == "Admitted" || s == "Active" || s == "Promoted" || s == "Repeating" || s == "Transferred" || s == "Graduated" || s == "Dropped" || s == "Archived")
                .WithMessage("Invalid Student Status.");
        }
    }

    public class UpdateStudentRequestValidator : AbstractValidator<UpdateStudentRequest>
    {
        public UpdateStudentRequestValidator()
        {
            RuleFor(x => x.AdmissionNo)
                .NotEmpty().WithMessage("Admission number is required.")
                .MaximumLength(50).WithMessage("Admission number cannot exceed 50 characters.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100).WithMessage("First name cannot exceed 100 characters.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100).WithMessage("Last name cannot exceed 100 characters.");

            RuleFor(x => x.DateOfBirth)
                .NotEmpty().WithMessage("Date of birth is required.")
                .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past.");

            RuleFor(x => x.Gender)
                .NotEmpty().WithMessage("Gender is required.")
                .MaximumLength(10).WithMessage("Gender cannot exceed 10 characters.");

            RuleFor(x => x.AdmissionDate)
                .NotEmpty().WithMessage("Admission date is required.");

            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("Class Standard ID is required.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "Applied" || s == "Admitted" || s == "Active" || s == "Promoted" || s == "Repeating" || s == "Transferred" || s == "Graduated" || s == "Dropped" || s == "Archived")
                .WithMessage("Invalid Student Status.");
        }
    }
}
