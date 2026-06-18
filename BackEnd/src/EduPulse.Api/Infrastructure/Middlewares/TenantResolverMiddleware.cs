using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using EduPulse.Core.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace EduPulse.Api.Infrastructure.Middlewares
{
    public class TenantResolverMiddleware
    {
        private readonly RequestDelegate _next;

        public TenantResolverMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, ITenantContext tenantContext)
        {
            var user = context.User;
            if (user?.Identity?.IsAuthenticated == true)
            {
                var tenantIdClaim = user.FindFirst("TenantId")?.Value;
                var userIdClaim = user.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) 
                    ?? user.FindFirst(JwtRegisteredClaimNames.Sub);
                var emailClaim = user.FindFirst(System.Security.Claims.ClaimTypes.Email) 
                    ?? user.FindFirst(JwtRegisteredClaimNames.Email);

                if (Guid.TryParse(tenantIdClaim, out var tenantId) && 
                    Guid.TryParse(userIdClaim?.Value, out var userId))
                {
                    tenantContext.SetContext(tenantId, userId, emailClaim?.Value ?? string.Empty);
                }
            }
            else
            {
                if (context.Request.Headers.TryGetValue("X-Tenant-Id", out var tenantHeaderValues))
                {
                    var headerValue = tenantHeaderValues.FirstOrDefault();
                    if (Guid.TryParse(headerValue, out var tenantId))
                    {
                        tenantContext.SetContext(tenantId, Guid.Empty, string.Empty);
                    }
                }
            }

            await _next(context);
        }
    }
}
