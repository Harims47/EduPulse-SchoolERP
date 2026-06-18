using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Academics;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IClassService
    {
        Task<IEnumerable<ClassDto>> GetAllAsync();
        Task<ClassDto?> GetByIdAsync(Guid id);
        Task<ClassDto> CreateAsync(CreateClassRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateClassRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
