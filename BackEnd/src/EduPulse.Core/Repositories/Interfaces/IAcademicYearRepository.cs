using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IAcademicYearRepository
    {
        Task<IEnumerable<AcademicYear>> GetAllAsync(Guid tenantId);
        Task<AcademicYear?> GetByIdAsync(Guid tenantId, Guid academicYearId);
        Task<bool> ExistsByNameAsync(Guid tenantId, string name, Guid? excludeId = null);
        Task<Guid> CreateAsync(AcademicYear entity);
        Task<bool> UpdateAsync(AcademicYear entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid academicYearId, Guid deletedByUserId);
    }
}
