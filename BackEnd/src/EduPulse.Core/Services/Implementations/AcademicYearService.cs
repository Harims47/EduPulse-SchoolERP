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
    public class AcademicYearService : IAcademicYearService
    {
        private readonly IAcademicYearRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateAcademicYearRequest> _createValidator;
        private readonly IValidator<UpdateAcademicYearRequest> _updateValidator;

        public AcademicYearService(
            IAcademicYearRepository repository,
            ITenantContext tenantContext,
            IValidator<CreateAcademicYearRequest> createValidator,
            IValidator<UpdateAcademicYearRequest> updateValidator)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<AcademicYearDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => new AcademicYearDto(e.AcademicYearId, e.Name, e.StartDate, e.EndDate));
        }

        public async Task<AcademicYearDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return new AcademicYearDto(entity.AcademicYearId, entity.Name, entity.StartDate, entity.EndDate);
        }

        public async Task<AcademicYearDto> CreateAsync(CreateAcademicYearRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name))
            {
                throw new ValidationException($"Academic Year '{request.Name}' already exists.");
            }

            var entity = new AcademicYear
            {
                TenantId = tenantId,
                Name = request.Name,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            return new AcademicYearDto(id, entity.Name, entity.StartDate, entity.EndDate);
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateAcademicYearRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            if (await _repository.ExistsByNameAsync(tenantId, request.Name, id))
            {
                throw new ValidationException($"Academic Year '{request.Name}' already exists.");
            }

            existing.Name = request.Name;
            existing.StartDate = request.StartDate;
            existing.EndDate = request.EndDate;
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
