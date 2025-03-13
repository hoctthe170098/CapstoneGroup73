using StudyFlow.Application.Common.Interfaces;
using StudyFlow.Domain.Constants;
using StudyFlow.Infrastructure.Data;
using StudyFlow.Infrastructure.Data.Interceptors;
using StudyFlow.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        Guard.Against.Null(connectionString, message: "Connection string 'DefaultConnection' not found.");

        services.AddScoped<ISaveChangesInterceptor, AuditableEntityInterceptor>();
        services.AddScoped<ISaveChangesInterceptor, DispatchDomainEventsInterceptor>();

        services.AddDbContext<ApplicationDbContext>((sp, options) =>
        {
            options.AddInterceptors(sp.GetServices<ISaveChangesInterceptor>());

            options.UseSqlServer(connectionString);
        });

        services.AddScoped<IApplicationDbContext>(provider => provider.GetRequiredService<ApplicationDbContext>());

        services.AddScoped<ApplicationDbContextInitialiser>();

        services
            .AddDefaultIdentity<ApplicationUser>()
            .AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>();

        services.AddSingleton(TimeProvider.System);
        // Sửa ở đây: Đăng ký IdentityService với IConfiguration
        services.AddTransient<IIdentityService, IdentityService>(provider =>
            new IdentityService(
                provider.GetRequiredService<UserManager<ApplicationUser>>(),
                provider.GetRequiredService<IUserClaimsPrincipalFactory<ApplicationUser>>(),
                provider.GetRequiredService<IAuthorizationService>(),
                configuration,provider.GetRequiredService<IApplicationDbContext>())); // Truyền configuration vào constructor

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanCreateCoso, policy => policy.RequireRole(Roles.Administrator));
            options.AddPolicy(Policies.CanPurge, policy => policy.RequireRole(Roles.Administrator));
            // Policy cho quyền của tất cả người dùng đã đăng nhập
            options.AddPolicy(Policies.AuthenticatedUser, policy => policy.RequireAuthenticatedUser());
        });
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] ?? "")) // Thêm kiểm tra null
                };
            });
        services.AddTransient<IIdentityService, IdentityService>();

        services.AddAuthorization(options =>
        {
            options.AddPolicy(Policies.CanPurge, policy 
                => policy.RequireRole(Roles.Administrator));
        }
          );
       
        return services;
    }
}
