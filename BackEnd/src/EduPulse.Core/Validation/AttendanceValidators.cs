using FluentValidation;
using EduPulse.Core.Dtos.Attendance;
using System;

namespace EduPulse.Core.Validation
{
    public class MarkAttendanceRequestValidator : AbstractValidator<MarkAttendanceRequest>
    {
        public MarkAttendanceRequestValidator()
        {
            RuleFor(x => x.ClassId)
                .NotEmpty().WithMessage("Class ID is required.");

            RuleFor(x => x.SectionId)
                .NotEmpty().WithMessage("Section ID is required.");

            RuleFor(x => x.Date)
                .NotEmpty().WithMessage("Date is required.")
                .LessThanOrEqualTo(DateTime.Today).WithMessage("Date cannot be in the future.");

            RuleFor(x => x.Entries)
                .NotEmpty().WithMessage("Attendance entries list cannot be empty.");

            RuleForEach(x => x.Entries).SetValidator(new AttendanceEntryRequestValidator());
        }
    }

    public class AttendanceEntryRequestValidator : AbstractValidator<AttendanceEntryRequest>
    {
        public AttendanceEntryRequestValidator()
        {
            RuleFor(x => x.StudentId)
                .NotEmpty().WithMessage("Student ID is required.");

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .Must(s => s == "P" || s == "A" || s == "L" || s == "T")
                .WithMessage("Invalid status. Must be 'P' (Present), 'A' (Absent), 'L' (Late), or 'T' (HalfDay/Tardy).");

            RuleFor(x => x.Remarks)
                .MaximumLength(200).WithMessage("Remarks cannot exceed 200 characters.")
                .When(x => !string.IsNullOrEmpty(x.Remarks));
        }
    }
}
