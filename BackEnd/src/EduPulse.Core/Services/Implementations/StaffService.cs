using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Domain.Entities;
using EduPulse.Core.Dtos.Staff;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class StaffService : IStaffService
    {
        private readonly IStaffRepository _repository;
        private readonly ITenantContext _tenantContext;
        private readonly IValidator<CreateStaffRequest> _createValidator;
        private readonly IValidator<UpdateStaffRequest> _updateValidator;

        public StaffService(
            IStaffRepository repository,
            ITenantContext tenantContext,
            IValidator<CreateStaffRequest> createValidator,
            IValidator<UpdateStaffRequest> updateValidator)
        {
            _repository = repository;
            _tenantContext = tenantContext;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        public async Task<IEnumerable<StaffDto>> GetAllAsync()
        {
            var tenantId = _tenantContext.TenantId;
            var entities = await _repository.GetAllAsync(tenantId);
            return entities.Select(e => new StaffDto(
                e.StaffId,
                e.UserId,
                e.EmployeeCode,
                e.FirstName,
                e.LastName,
                e.Phone,
                e.Designation,
                e.PhotoPath,
                e.IsActive
            ));
        }

        public async Task<StaffDto?> GetByIdAsync(Guid id)
        {
            var tenantId = _tenantContext.TenantId;
            var entity = await _repository.GetByIdAsync(tenantId, id);
            if (entity == null) return null;

            return new StaffDto(
                entity.StaffId,
                entity.UserId,
                entity.EmployeeCode,
                entity.FirstName,
                entity.LastName,
                entity.Phone,
                entity.Designation,
                entity.PhotoPath,
                entity.IsActive
            );
        }

        public async Task<StaffDto> CreateAsync(CreateStaffRequest request)
        {
            await _createValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            if (await _repository.ExistsByEmployeeCodeAsync(tenantId, request.EmployeeCode))
            {
                throw new ValidationException($"Employee code '{request.EmployeeCode}' is already registered.");
            }

            var entity = new Staff
            {
                TenantId = tenantId,
                UserId = request.UserId,
                EmployeeCode = request.EmployeeCode,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Phone = request.Phone,
                Designation = request.Designation,
                PhotoPath = request.PhotoPath,
                IsActive = request.IsActive,
                IsDeleted = false,
                CreatedOn = DateTime.UtcNow,
                CreatedByUserId = _tenantContext.UserId
            };

            var id = await _repository.CreateAsync(entity);
            return new StaffDto(
                id,
                entity.UserId,
                entity.EmployeeCode,
                entity.FirstName,
                entity.LastName,
                entity.Phone,
                entity.Designation,
                entity.PhotoPath,
                entity.IsActive
            );
        }

        public async Task<bool> UpdateAsync(Guid id, UpdateStaffRequest request)
        {
            await _updateValidator.ValidateAndThrowAsync(request);

            var tenantId = _tenantContext.TenantId;

            var existing = await _repository.GetByIdAsync(tenantId, id);
            if (existing == null) return false;

            if (await _repository.ExistsByEmployeeCodeAsync(tenantId, request.EmployeeCode, id))
            {
                throw new ValidationException($"Employee code '{request.EmployeeCode}' is already registered.");
            }

            existing.UserId = request.UserId;
            existing.EmployeeCode = request.EmployeeCode;
            existing.FirstName = request.FirstName;
            existing.LastName = request.LastName;
            existing.Phone = request.Phone;
            existing.Designation = request.Designation;
            existing.PhotoPath = request.PhotoPath;
            existing.IsActive = request.IsActive;
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
