using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface ISectionRepository
    {
        Task<IEnumerable<Section>> GetAllAsync(Guid tenantId);
        Task<Section?> GetByIdAsync(Guid tenantId, Guid sectionId);
        Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
        Task<Guid> CreateAsync(Section entity);
        Task<bool> UpdateAsync(Section entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid sectionId, Guid deletedByUserId);
    }
}
