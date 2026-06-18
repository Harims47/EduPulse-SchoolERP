using System;
using EduPulse.Core.Common.Interfaces;

namespace EduPulse.Core.Common.Implementations
{
    public class TenantContext : ITenantContext
    {
        public Guid TenantId { get; private set; } = Guid.Empty;
        public Guid UserId { get; private set; } = Guid.Empty;
        public string Email { get; private set; } = string.Empty;

        public void SetContext(Guid tenantId, Guid userId, string email)
        {
            TenantId = tenantId;
            UserId = userId;
            Email = email;
        }
    }
}
