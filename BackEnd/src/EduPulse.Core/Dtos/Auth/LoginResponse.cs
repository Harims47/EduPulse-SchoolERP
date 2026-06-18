using System;
using System.Collections.Generic;

namespace EduPulse.Core.Dtos.Auth
{
    public record LoginResponse(
        string Token, 
        Guid UserId, 
        Guid TenantId, 
        string Email, 
        List<string> Roles
    );
}
