using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Students;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _repository;
        private readonly IClassRepository _classRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateStudentRequest> _createValidator;
        private readonly IValidator<UpdateStudentRequest> _updateValidator;

        public StudentService(
            IStudentRepository repository,
            IClassRepository classRepository,
            ISectionRepository sectionRepository,
            ITenantContext tenantContext,
            IValidator<CreateStudentRequest> createValidator,
            IValidator<UpdateStudentRequest> updateValidator)
        {
            _repository = repository;
            _classRepository = classRepository;
            _sectionRepository = sectionRepository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<StudentDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => MapToDto(e));
        }

        public async Task<StudentDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return MapToDto(entity);
        }

        public async Task<StudentDto> CreateAsync(CreateStudentRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            // 1. Verify Class and Section exist and belong to the same tenant
            var classObj = await _classRepository.GetByIdAsync(tenantId, request.ClassId);
            if (classObj == null)
            {
                throw new ValidationException($"Class standard with ID '{request.ClassId}' was not found.");
            }

            var sectionObj = await _sectionRepository.GetByIdAsync(tenantId, request.SectionId);
            if (sectionObj == null)
            {
                throw new ValidationException($"Section with ID '{request.SectionId}' was not found.");
            }

            // 2. Check duplicate admission number
            if (await _repository.ExistsByAdmissionNoAsync(tenantId, request.AdmissionNo))
            {
                throw new ValidationException($"Admission number '{request.AdmissionNo}' already exists.");
            }

            var entity = new Student
            {
                TenantId = tenantId,
                AdmissionNo = request.AdmissionNo,
                FirstName = request.FirstName,
                LastName = request.LastName,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                BloodGroup = request.BloodGroup,
                GovernmentIdType = request.GovernmentIdType,
                GovernmentIdNumber = request.GovernmentIdNumber,
                SocialCategory = request.SocialCategory,
                PhotoPath = request.PhotoPath,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                Pincode = request.Pincode,
                AdmissionDate = request.AdmissionDate,
                Status = request.Status,
                ClassId = request.ClassId,
                SectionId = request.SectionId,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            entity.StudentId = id;
            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateStudentRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            // 1. Verify Class and Section exist
            var classObj = await _classRepository.GetByIdAsync(tenantId, request.ClassId);
            if (classObj == null)
            {
                throw new ValidationException($"Class standard with ID '{request.ClassId}' was not found.");
            }

            var sectionObj = await _sectionRepository.GetByIdAsync(tenantId, request.SectionId);
            if (sectionObj == null)
            {
                throw new ValidationException($"Section with ID '{request.SectionId}' was not found.");
            }

            // 2. Check duplicate admission number
            if (await _repository.ExistsByAdmissionNoAsync(tenantId, request.AdmissionNo, id))
            {
                throw new ValidationException($"Admission number '{request.AdmissionNo}' already exists.");
            }

            existing.AdmissionNo = request.AdmissionNo;
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.DateOfBirth = request.DateOfBirth;
            existing.Gender = request.Gender;
            existing.BloodGroup = request.BloodGroup;
            existing.GovernmentIdType = request.GovernmentIdType;
            existing.GovernmentIdNumber = request.GovernmentIdNumber;
            existing.SocialCategory = request.SocialCategory;
            existing.PhotoPath = request.PhotoPath;
            existing.AddressLine1 = request.AddressLine1;
            existing.AddressLine2 = request.AddressLine2;
            existing.City = request.City;
            existing.State = request.State;
            existing.Pincode = request.Pincode;
            existing.AdmissionDate = request.AdmissionDate;
            existing.Status = request.Status;
            existing.ClassId = request.ClassId;
            existing.SectionId = request.SectionId;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedByUserId = _tenantContext.UserId;

            return await _repository.UpdateAsync(existing);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            
            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            return await _repository.SoftDeleteAsync(tenantId, id, _tenantContext.UserId);
        }

        private static StudentDto MapToDto(Student e)
        {
            return new StudentDto(
                e.StudentId,
                e.AdmissionNo,
                e.FirstName,
                e.LastName,
                e.DateOfBirth,
                e.Gender,
                e.BloodGroup,
                e.GovernmentIdType,
                e.GovernmentIdNumber,
                e.SocialCategory,
                e.PhotoPath,
                e.AddressLine1,
                e.AddressLine2,
                e.City,
                e.State,
                e.Pincode,
                e.AdmissionDate,
                e.Status,
                e.ClassId,
                e.SectionId
            );
        }
    }
}
