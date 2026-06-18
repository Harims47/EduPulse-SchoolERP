using System.Collections.Generic;
using EduPulse.Core.Domain.Entities;

namespace EduPulse.Core.Services.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user, List<string> roles);
    }
}
