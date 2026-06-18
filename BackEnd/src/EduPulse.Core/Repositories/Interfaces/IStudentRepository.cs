using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<IEnumerable<Student>> GetAllAsync(Guid tenantId);
        Task<Student?> GetByIdAsync(Guid tenantId, Guid studentId);
        Task<bool> ExistsByAdmissionNoAsync(Guid tenantId, string admissionNo, Guid? excludeId = null);
        Task<Guid> CreateAsync(Student entity);
        Task<bool> UpdateAsync(Student entity);
        Task<bool> SoftDeleteAsync(Guid tenantId, Guid studentId, Guid deletedByUserId);
    }
}
