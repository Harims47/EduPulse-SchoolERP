using System.Threading.Tasks;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<UserWithRoles?> GetByEmailAsync(string email);
    }
}
