using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Notifications.Commands.InactivityNotification;
using Fliq.Contracts.Notifications.Helpers;
using MediatR;
using Microsoft.Extensions.Logging;
using Quartz;

namespace Fliq.Infrastructure.Services.SchedulingServices.QuartzJobs
{
    public class InactivityCheckJob : IJob
    {

        private readonly IUserRepository _userRepository;
        private readonly IUserFeatureActivityRepository _featureActivityRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<InactivityCheckJob> _logger;

        private readonly TimeSpan _generalInactivityThreshold = TimeSpan.FromDays(30);
        private readonly TimeSpan _featureInactivityThreshold = TimeSpan.FromDays(7);

        public InactivityCheckJob(
            IUserRepository userRepository,
            IUserFeatureActivityRepository featureActivityRepository,
            IMediator mediator,
            ILogger<InactivityCheckJob> logger)
        {
            _userRepository = userRepository;
            _featureActivityRepository = featureActivityRepository;
            _mediator = mediator;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("Inactivity Check Job Started at {Time}", DateTime.UtcNow);

            var now = DateTime.UtcNow;

            // General Inactivity Check
            var inactiveUsers = await _userRepository.GetInactiveUsersAsync(now - _generalInactivityThreshold);
            foreach (var user in inactiveUsers)
            {
                _logger.LogInformation("Sending general inactivity notification to User {UserId}", user.Id);

                var message = NotificationMessageHelper.GetGeneralInactivityMessage(); ;
                await _mediator.Send(new SendInactivityNotificationCommand(user.Id, message));
            }

            // Feature-Specific Inactivity Check
            var inactiveFeatures = await _featureActivityRepository.GetInactiveFeatureUsersAsync(now - _featureInactivityThreshold);
            foreach (var featureGroup in inactiveFeatures)
            {
                _logger.LogInformation("Sending feature inactivity notification to User {UserId} for Feature {Feature}", featureGroup.UserId, featureGroup.Feature);

                var message = NotificationMessageHelper.GetFeatureInactivityMessage(featureGroup.Feature);
                await _mediator.Send(new SendInactivityNotificationCommand(featureGroup.UserId, message));
            }

            _logger.LogInformation("Inactivity Check Job Completed.");
        }
    }
}
