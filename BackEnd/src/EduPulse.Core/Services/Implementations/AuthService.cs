using System.Threading.Tasks;
using BCrypt.Net;
using EduPulse.Core.Dtos.Auth;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Services.Interfaces;

namespace EduPulse.Core.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IJwtTokenService _jwtTokenService;

        public AuthService(IUserRepository userRepository, IJwtTokenService jwtTokenService)
        {
            _userRepository = userRepository;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var userWithRoles = await _userRepository.GetByEmailAsync(request.Email);
            if (userWithRoles == null)
            {
                return null;
            }

            var user = userWithRoles.User;

            if (!user.IsActive)
            {
                return null; 
            }

            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return null;
            }

            var token = _jwtTokenService.GenerateToken(user, userWithRoles.Roles);

            return new LoginResponse(
                token,
                user.UserId,
                user.TenantId,
                user.Email,
                userWithRoles.Roles
            );
        }
    }
}
