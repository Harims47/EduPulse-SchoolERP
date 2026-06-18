using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Students;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IStudentGuardianService
    {
        Task<IEnumerable<StudentGuardianDto>> GetGuardiansByStudentIdAsync(Guid studentId);
        Task<bool> LinkGuardianAsync(Guid studentId, LinkGuardianRequest request);
        Task<bool> UpdateRelationshipAsync(Guid studentId, Guid guardianId, UpdateRelationshipRequest request);
        Task<bool> UnlinkGuardianAsync(Guid studentId, Guid guardianId);
    }
}
