using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Staff;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IStaffService
    {
        Task<IEnumerable<StaffDto>> GetAllAsync();
        Task<StaffDto?> GetByIdAsync(Guid id);
        Task<StaffDto> CreateAsync(CreateStaffRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateStaffRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
