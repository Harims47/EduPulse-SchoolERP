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
    public class StudentGuardianService : IStudentGuardianService
    {
        private readonly IStudentGuardianRepository _repository;
        private readonly IStudentRepository _studentRepository;
        private readonly IGuardianRepository _guardianRepository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<LinkGuardianRequest> _linkValidator;
        private readonly IValidator<UpdateRelationshipRequest> _updateValidator;

        public StudentGuardianService(
            IStudentGuardianRepository repository,
            IStudentRepository studentRepository,
            IGuardianRepository guardianRepository,
            ITenantContext tenantContext,
            IValidator<LinkGuardianRequest> linkValidator,
            IValidator<UpdateRelationshipRequest> updateValidator)
        {
            _repository = repository;
            _studentRepository = studentRepository;
            _guardianRepository = guardianRepository;
            _tenantContext = tenantContext;
            _linkValidator = linkValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<StudentGuardianDto>> GetGuardiansByStudentIdAsync(Guid studentId)
        {
            var tenantId = _tenantContext.TenantId;

            // Verify student exists
            var student = await _studentRepository.GetByIdAsync(tenantId, studentId);
            if (student == null)
            {
                throw new ValidationException($"Student with ID '{studentId}' was not found.");
            }

            var relations = await _repository.GetGuardiansByStudentIdAsync(tenantId, studentId);
            
            var result = new List<StudentGuardianDto>();
            foreach (var relation in relations)
            {
                var guardian = await _guardianRepository.GetByIdAsync(tenantId, relation.GuardianId);
                if (guardian != null)
                {
                    result.Add(new StudentGuardianDto(
                        relation.StudentId,
                        relation.GuardianId,
                        relation.RelationshipType,
                        relation.IsPrimaryContact,
                        relation.IsBillingResponsible,
                        guardian.FirstName,
                        guardian.LastName,
                        guardian.Phone
                    ));
                }
            }

            return result;
        }

        public async Task<bool> LinkGuardianAsync(Guid studentId, LinkGuardianRequest request)
        {
            await _linkValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            // 1. Verify student exists
            var student = await _studentRepository.GetByIdAsync(tenantId, studentId);
            if (student == null)
            {
                throw new ValidationException($"Student with ID '{studentId}' was not found.");
            }

            // 2. Verify guardian exists
            var guardian = await _guardianRepository.GetByIdAsync(tenantId, request.GuardianId);
            if (guardian == null)
            {
                throw new ValidationException($"Guardian with ID '{request.GuardianId}' was not found.");
            }

            // 3. Check if relationship already exists
            var existing = await _repository.GetRelationAsync(tenantId, studentId, request.GuardianId);
            if (existing != null)
            {
                throw new ValidationException("This relationship already exists.");
            }

            var entity = new StudentGuardian
            {
                StudentId = studentId,
                GuardianId = request.GuardianId,
                TenantId = tenantId,
                RelationshipType = request.RelationshipType,
                IsPrimaryContact = request.IsPrimaryContact,
                IsBillingResponsible = request.IsBillingResponsible,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            return await _repository.CreateRelationAsync(entity);
        }

        public async Task<bool> UpdateRelationshipAsync(Guid studentId, Guid guardianId, UpdateRelationshipRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetRelationAsync(tenantId, studentId, guardianId);
            if (existing == null) return false;

            existing.RelationshipType = request.RelationshipType;
            existing.IsPrimaryContact = request.IsPrimaryContact;
            existing.IsBillingResponsible = request.IsBillingResponsible;
            existing.ModifiedOn = DateTime.UtcNow;
            existing.ModifiedByUserId = _tenantContext.UserId;

            return await _repository.UpdateRelationAsync(existing);
        }

        public async Task<bool> UnlinkGuardianAsync(Guid studentId, Guid guardianId)
        {
            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetRelationAsync(tenantId, studentId, guardianId);
            if (existing == null) return false;

            return await _repository.DeleteRelationAsync(tenantId, studentId, guardianId, _tenantContext.UserId);
        }
    }
}
