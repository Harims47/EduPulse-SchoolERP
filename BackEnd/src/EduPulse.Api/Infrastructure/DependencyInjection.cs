using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using EduPulse.Core.Common.Interfaces;
using EduPulse.Core.Common.Implementations;
using EduPulse.Core.Database.Connection;
using EduPulse.Core.Repositories.Interfaces;
using EduPulse.Core.Repositories.Implementations;
using EduPulse.Core.Services.Interfaces;
using EduPulse.Core.Services.Implementations;
using EduPulse.Core.Dtos.Academics;
using EduPulse.Core.Validation;
using EduPulse.Api.Infrastructure.Authentication;

namespace EduPulse.Api.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddAuthenticationModule(this IServiceCollection services)
        {
            // Database Connection
            services.AddScoped<IDbConnectionFactory, SqlConnectionFactory>();

            // Scoped Tenant Context
            services.AddScoped<ITenantContext, TenantContext>();

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();

            // Services
            services.AddScoped<IJwtTokenService, JwtTokenService>();
            services.AddScoped<IAuthService, AuthService>();

            return services;
        }

        public static IServiceCollection AddAcademicsModule(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();

            // Services
            services.AddScoped<IAcademicYearService, AcademicYearService>();

            // Validators
            services.AddTransient<IValidator<CreateAcademicYearRequest>, CreateAcademicYearRequestValidator>();
            services.AddTransient<IValidator<UpdateAcademicYearRequest>, UpdateAcademicYearRequestValidator>();

            return services;
        }
    }
}
