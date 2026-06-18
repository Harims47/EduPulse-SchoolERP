using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<IEnumerable<Class>> GetAllAsync(Guid tenantId);
        Task<Class?> GetByIdAsync(Guid tenantId, Guid classId);
        Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
        Task<Guid> CreateAsync(Class entity);
        Task<bool> UpdateAsync(Class entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid classId, Guid deletedByUserId);
    }
}
