using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Guardians;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IGuardianService
    {
        Task<IEnumerable<GuardianDto>> GetAllAsync();
        Task<GuardianDto?> GetByIdAsync(Guid id);
        Task<GuardianDto> CreateAsync(CreateGuardianRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateGuardianRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
