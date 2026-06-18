using System;

namespace EduPulse.Core.Common.Interfaces
{
    public interface ITenantContext
    {
        Guid TenantId { get; }
        Guid UserId { get; }
        string Email { get; }
        void SetContext(Guid tenantId, Guid userId, string email);
    }
}
