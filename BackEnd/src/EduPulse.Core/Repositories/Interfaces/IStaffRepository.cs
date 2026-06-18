using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IStaffRepository
    {
        Task<IEnumerable<Staff>> GetAllAsync(Guid tenantId);
        Task<Staff?> GetByIdAsync(Guid tenantId, Guid staffId);
        Task<bool> ExistsByEmployeeCodeAsync(Guid tenantId, string employeeCode, Guid? excludeId = null);
        Task<Guid> CreateAsync(Staff entity);
        Task<bool> UpdateAsync(Staff entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid staffId, Guid deletedByUserId);
    }
}
