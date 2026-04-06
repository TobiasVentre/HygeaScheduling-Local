using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using SchedulingMS.Api.Security;

namespace SchedulingMS.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "SchedulingMS",
                Version = "v1",
                Description = "Scheduling and availability microservice"
            });

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "JWT Authorization header using the Bearer scheme."
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
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

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            var rawKey = configuration["JwtSettings:key"];
            if (string.IsNullOrWhiteSpace(rawKey))
            {
                throw new InvalidOperationException("No se encontró 'JwtSettings:key' para SchedulingMS.");
            }

            var keyBytes = Encoding.UTF8.GetBytes(rawKey);
            if (keyBytes.Length < 32)
            {
                keyBytes = SHA256.HashData(keyBytes);
            }

            options.RequireHttpsMetadata = false;
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero,
                NameClaimType = ClaimTypes.NameIdentifier,
                RoleClaimType = ClaimTypes.Role,
                IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
            };
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy(SecurityConstants.Policies.AdminOnly, policy =>
                policy.RequireRole(SecurityConstants.Roles.Admin));
            options.AddPolicy(SecurityConstants.Policies.ProviderAdminOrAdmin, policy =>
                policy.RequireRole(SecurityConstants.Roles.ProviderAdmin, SecurityConstants.Roles.Admin));
            options.AddPolicy(SecurityConstants.Policies.TechnicianOnly, policy =>
                policy.RequireRole(SecurityConstants.Roles.Technician));
            options.AddPolicy(SecurityConstants.Policies.ClientOnly, policy =>
                policy.RequireRole(SecurityConstants.Roles.Client));
        });

        services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });

        return services;
    }
}


