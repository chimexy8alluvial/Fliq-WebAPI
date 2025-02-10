using Fliq.Application.Common.Behaviours;
using Fliq.Application.Common.Interfaces.UserFeatureActivities;
using Fliq.Application.Common.UserFeatureActivities;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Fliq.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly));
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehaviour<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddSignalR();
            services.AddScoped<IUserFeatureActivityService, UserFeatureActivityService>();
            return services;
        }
    }
}