using System.Threading.Tasks;
using EduPulse.Core.Dtos.Auth;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IAuthService
    {
        Task<LoginResponse?> LoginAsync(LoginRequest request);
    }
}
