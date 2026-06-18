using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IStudentGuardianRepository
    {
        Task<IEnumerable<StudentGuardian>> GetGuardiansByStudentIdAsync(Guid tenantId, Guid studentId);
        Task<StudentGuardian?> GetRelationAsync(Guid tenantId, Guid studentId, Guid guardianId);
        Task<bool> CreateRelationAsync(StudentGuardian entity);
        Task<bool> UpdateRelationAsync(StudentGuardian entity);
        Task<bool> DeleteRelationAsync(Guid tenantId, Guid studentId, Guid guardianId, Guid deletedByUserId);
    }
}
