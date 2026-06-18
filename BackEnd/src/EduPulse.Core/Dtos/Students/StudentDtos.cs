using System;

namespace EduPulse.Core.Dtos.Students
{
    public record StudentDto(
        Guid StudentId,
        string AdmissionNo,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string Gender,
        string? BloodGroup,
        string? GovernmentIdType,
        string? GovernmentIdNumber,
        string? SocialCategory,
        string? PhotoPath,
        string? AddressLine1,
        string? AddressLine2,
        string? City,
        string? State,
        string? Pincode,
        DateTime AdmissionDate,
        string Status,
        Guid ClassId,
        Guid SectionId
    );

    public record CreateStudentRequest(
        string AdmissionNo,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string Gender,
        string? BloodGroup,
        string? GovernmentIdType,
        string? GovernmentIdNumber,
        string? SocialCategory,
        string? PhotoPath,
        string? AddressLine1,
        string? AddressLine2,
        string? City,
        string? State,
        string? Pincode,
        DateTime AdmissionDate,
        string Status,
        Guid ClassId,
        Guid SectionId
    );

    public record UpdateStudentRequest(
        string AdmissionNo,
        string FirstName,
        string LastName,
        DateTime DateOfBirth,
        string Gender,
        string? BloodGroup,
        string? GovernmentIdType,
        string? GovernmentIdNumber,
        string? SocialCategory,
        string? PhotoPath,
        string? AddressLine1,
        string? AddressLine2,
        string? City,
        string? State,
        string? Pincode,
        DateTime AdmissionDate,
        string Status,
        Guid ClassId,
        Guid SectionId
    );
}
