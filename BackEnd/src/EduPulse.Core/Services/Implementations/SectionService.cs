using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Academics;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class SectionService : ISectionService
    {
        private readonly ISectionRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateSectionRequest> _createValidator;
        private readonly IValidator<UpdateSectionRequest> _updateValidator;

        public SectionService(
            ISectionRepository repository,
            ITenantContext tenantContext,
            IValidator<CreateSectionRequest> createValidator,
            IValidator<UpdateSectionRequest> updateValidator)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<SectionDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => new SectionDto(e.SectionId, e.Name));
        }

        public async Task<SectionDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return new SectionDto(entity.SectionId, entity.Name);
        }

        public async Task<SectionDto> CreateAsync(CreateSectionRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name))
            {
                throw new ValidationException($"Section '{request.Name}' already exists.");
            }

            var entity = new Section
            {
                TenantId = tenantId,
                Name = request.Name,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            return new SectionDto(id, entity.Name);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateSectionRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name, id))
            {
                throw new ValidationException($"Section '{request.Name}' already exists.");
            }

            existing.Name = request.Name;
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
    }
}
