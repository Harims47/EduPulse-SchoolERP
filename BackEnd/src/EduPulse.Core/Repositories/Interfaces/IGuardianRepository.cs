using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IGuardianRepository
    {
        Task<IEnumerable<Guardian>> GetAllAsync(Guid tenantId);
        Task<Guardian?> GetByIdAsync(Guid tenantId, Guid guardianId);
        Task<bool> ExistsByPhoneAsync(Guid tenantId, string phone, Guid? excludeId = null);
        Task<Guid> CreateAsync(Guardian entity);
        Task<bool> UpdateAsync(Guardian entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid guardianId, Guid deletedByUserId);
    }
}
