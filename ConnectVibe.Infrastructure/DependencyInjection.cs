using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Infrastructure.Authentication;
using ConnectVibe.Infrastructure.Persistence;
using ConnectVibe.Infrastructure.Persistence.Repositories;
using ConnectVibe.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ConnectVibe.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configurationManager)
        {
            services.AddAuth(configurationManager);
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IOtpRepository, OtpRepository>();
            services.AddSingleton<IEmailService, EmailService>();
            services.AddSingleton<IOtpService, OtpService>();
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();
            services.AddDbContext<ConnectVibeDbContext>(options =>
    options.UseSqlServer(configurationManager.GetConnectionString("ConnectVibeDbContext") ?? throw new InvalidOperationException("Connection string 'ConnectVibeDbContext' not found.")));
            return services;
        }
        public static IServiceCollection AddAuth(this IServiceCollection services, ConfigurationManager configurationManager)
        {

            services.Configure<JwtSettings>(configurationManager.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

            var jwtSettings = new JwtSettings();
            configurationManager.GetSection(JwtSettings.SectionName).Bind(jwtSettings);
            services.AddAuthentication(defaultScheme: JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(
                options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),

                });
            return services;
        }

    }
}
