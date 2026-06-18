using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Guardians;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class GuardianService : IGuardianService
    {
        private readonly IGuardianRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateGuardianRequest> _createValidator;
        private readonly IValidator<UpdateGuardianRequest> _updateValidator;

        public GuardianService(
            IGuardianRepository repository,
            ITenantContext tenantContext,
            IValidator<CreateGuardianRequest> createValidator,
            IValidator<UpdateGuardianRequest> updateValidator)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<GuardianDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => MapToDto(e));
        }

        public async Task<GuardianDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return MapToDto(entity);
        }

        public async Task<GuardianDto> CreateAsync(CreateGuardianRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            // Check duplicate phone number in tenant
            if (await _repository.ExistsByPhoneAsync(tenantId, request.Phone))
            {
                throw new ValidationException($"Guardian with phone number '{request.Phone}' already exists.");
            }

            var entity = new Guardian
            {
                TenantId = tenantId,
                UserId = request.UserId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Email = request.Email,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            entity.GuardianId = id;
            return MapToDto(entity);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateGuardianRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            // Check duplicate phone number
            if (await _repository.ExistsByPhoneAsync(tenantId, request.Phone, id))
            {
                throw new ValidationException($"Guardian with phone number '{request.Phone}' already exists.");
            }

            existing.UserId = request.UserId;
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Phone = request.Phone;
            existing.Email = request.Email;
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

        private static GuardianDto MapToDto(Guardian e)
        {
            return new GuardianDto(
                e.GuardianId,
                e.UserId,
                e.FirstName,
                e.LastName,
                e.Phone,
                e.Email
            );
        }
    }
}
