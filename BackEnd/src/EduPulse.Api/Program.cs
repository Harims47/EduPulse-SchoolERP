using System;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using EduPulse.Api.Infrastructure;
using EduPulse.Api.Infrastructure.Middlewares;

var builder = WebApplication.CreateBuilder(args);

// 1. Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// 2. Configure Controllers with JSON CamelCase Options
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.WriteIndented = true;
    });

// 3. Configure Swagger with JWT Security Scheme
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "EduPulse AI API", 
        Version = "v1",
        Description = "Multi-Tenant School Management ERP SaaS API"
    });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter your token in the input field below (Swagger will prepand 'Bearer ' automatically).",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// 4. Configure JWT Authentication
var secretKey = builder.Configuration["Jwt:SecretKey"] 
    ?? throw new InvalidOperationException("Jwt:SecretKey configuration is missing.");
var issuer = builder.Configuration["Jwt:Issuer"] ?? "EduPulseApi";
var audience = builder.Configuration["Jwt:Audience"] ?? "EduPulseClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = builder.Environment.IsProduction();
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = issuer,
        ValidAudience = audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero
    };
});

// 5. Configure Authorization Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("SchoolAdminOnly", policy => policy.RequireRole("SchoolAdmin"));
    options.AddPolicy("AccountantOnly", policy => policy.RequireRole("Accountant"));
    options.AddPolicy("AdminOrAccountant", policy => policy.RequireRole("SchoolAdmin", "Accountant"));
});

// 6. Configure Health Checks
builder.Services.AddHealthChecks();

// 7. Register Module Dependencies
builder.Services.AddAuthenticationModule();
builder.Services.AddAcademicsModule();
builder.Services.AddStaffModule();

var app = builder.Build();

// 8. Configure Global Exception Handler Middleware
app.UseExceptionHandler(errorApp =>
{
    errorApp.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
        context.Response.ContentType = "application/json";

        var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
        var exception = exceptionHandlerPathFeature?.Error;

        var result = JsonSerializer.Serialize(new
        {
            statusCode = context.Response.StatusCode,
            message = "An unexpected error occurred. Please try again later.",
            detailedMessage = app.Environment.IsDevelopment() ? exception?.Message : null
        });

        await context.Response.WriteAsync(result);
    });
});

// 9. Configure Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EduPulse AI API v1");
        c.RoutePrefix = "swagger";
    });
}

// 10. Configure Middleware Pipelines
app.UseHttpsRedirection();
app.UseRouting();

app.UseCors("CorsPolicy");

app.UseAuthentication();
app.UseAuthorization();

// Resolve tenant context on authenticated routes
app.UseMiddleware<TenantResolverMiddleware>();

// 11. Configure API Routing & Health Check Endpoint
app.MapHealthChecks("/health");

app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();

app.Run();
