using Fliq.Api.Common.Errors;
using Fliq.Api.Mapping;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Fliq.Api
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPresentation(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddControllers();
            services.AddSingleton<ProblemDetailsFactory, FliqProblemDetailsFactory>();
            services.AddMappings();
            return services;
        }
    }
}