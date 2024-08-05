using ConnectVibe.Api.Common.Errors;
using ConnectVibe.Api.Mapping;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace ConnectVibe.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddSingleton<ProblemDetailsFactory, ConnectVibeProblemDetailsFactory>();
            services.AddMappings();
            return services;
        }
    }
}
