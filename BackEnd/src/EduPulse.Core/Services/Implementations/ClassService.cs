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
    public class ClassService : IClassService
    {
        private readonly IClassRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateClassRequest> _createValidator;
        private readonly IValidator<UpdateClassRequest> _updateValidator;

        public ClassService(
            IClassRepository repository,
            ITenantContext tenantContext,
            IValidator<CreateClassRequest> createValidator,
            IValidator<UpdateClassRequest> updateValidator)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<ClassDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => new ClassDto(e.ClassId, e.Name, e.SortOrder));
        }

        public async Task<ClassDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return new ClassDto(entity.ClassId, entity.Name, entity.SortOrder);
        }

        public async Task<ClassDto> CreateAsync(CreateClassRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name))
            {
                throw new ValidationException($"Class standard '{request.Name}' already exists.");
            }

            var entity = new Class
            {
                TenantId = tenantId,
                Name = request.Name,
                SortOrder = request.SortOrder,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            return new ClassDto(id, entity.Name, entity.SortOrder);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateClassRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name, id))
            {
                throw new ValidationException($"Class standard '{request.Name}' already exists.");
            }

            existing.Name = request.Name;
            existing.SortOrder = request.SortOrder;
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
