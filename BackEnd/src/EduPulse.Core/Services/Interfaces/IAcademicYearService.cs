using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Academics;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IAcademicYearService
    {
        Task<IEnumerable<AcademicYearDto>> GetAllAsync();
        Task<AcademicYearDto?> GetByIdAsync(Guid id);
        Task<AcademicYearDto> CreateAsync(CreateAcademicYearRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateAcademicYearRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
