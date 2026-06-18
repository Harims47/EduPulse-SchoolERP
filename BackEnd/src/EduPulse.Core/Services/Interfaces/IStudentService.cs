using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EduPulse.Core.Dtos.Students;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentDto>> GetAllAsync();
        Task<StudentDto?> GetByIdAsync(Guid id);
        Task<StudentDto> CreateAsync(CreateStudentRequest request);
        Task<bool> UpdateAsync(Guid id, UpdateStudentRequest request);
        Task<bool> DeleteAsync(Guid id);
    }
}
