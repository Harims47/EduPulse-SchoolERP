using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Academics;

namespace EduPulse.Core.Services.Interfaces
{
    public interface ISectionService
    {
        Task<IEnumerable<SectionDto>> GetAllAsync();
        Task<SectionDto?> GetByIdAsync(Guid id);
        Task<SectionDto> CreateAsync(CreateSectionRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateSectionRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
