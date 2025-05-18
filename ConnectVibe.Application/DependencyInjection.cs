using ErrorOr;
using Fliq.Application.Common.Behaviours;
using Fliq.Application.Common.Interfaces.UserFeatureActivities;
using Fliq.Application.Common.UserFeatureActivities;
using Fliq.Application.Contents.Commands;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.Prompts;
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
            // Register specific closed generic types
            services.AddTransient<IRequestHandler<ApproveContentCommand<PromptQuestion>, ErrorOr<Unit>>,
                ApproveContentCommandHandler<PromptQuestion>>();
            services.AddTransient<IRequestHandler<ApproveContentCommand<SpeedDatingEvent>, ErrorOr<Unit>>,
                ApproveContentCommandHandler<SpeedDatingEvent>>();
            services.AddTransient<IRequestHandler<ApproveContentCommand<BlindDate>, ErrorOr<Unit>>,
                ApproveContentCommandHandler<BlindDate>>();
            services.AddTransient<IRequestHandler<ApproveContentCommand<Game>, ErrorOr<Unit>>,
                ApproveContentCommandHandler<Game>>();
            services.AddTransient<IRequestHandler<ApproveContentCommand<Events>, ErrorOr<Unit>>,
                ApproveContentCommandHandler<Events>>();

            // Do the same for RejectContentCommand handlers
            services.AddTransient<IRequestHandler<RejectContentCommand<PromptQuestion>, ErrorOr<Unit>>,
                RejectContentCommandHandler<PromptQuestion>>();
            services.AddTransient<IRequestHandler<RejectContentCommand<SpeedDatingEvent>, ErrorOr<Unit>>,
              RejectContentCommandHandler<SpeedDatingEvent>>();
            services.AddTransient<IRequestHandler<RejectContentCommand<BlindDate>, ErrorOr<Unit>>,
                RejectContentCommandHandler<BlindDate>>();
            services.AddTransient<IRequestHandler<RejectContentCommand<Game>, ErrorOr<Unit>>,
                RejectContentCommandHandler<Game>>();
            services.AddTransient<IRequestHandler<RejectContentCommand<Events>, ErrorOr<Unit>>,
                RejectContentCommandHandler<Events>>();
            // ... add the rest similarly
            return services;
        }
    }
}