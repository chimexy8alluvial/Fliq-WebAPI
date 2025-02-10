using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Domain.Entities.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Fliq.Application.Notifications.Commands.InactivityNotification
{
    public record SendInactivityNotificationCommand(int UserId, string Message) : IRequest<Unit>;

    public class SendInactivityNotificationCommandHandler : IRequestHandler<SendInactivityNotificationCommand, Unit>
    {
        private readonly IEmailNotificationService _emailNotificationService;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SendInactivityNotificationCommandHandler> _logger;

        public SendInactivityNotificationCommandHandler(
            IEmailNotificationService emailNotificationService,
            IUserRepository userRepository,
            ILogger<SendInactivityNotificationCommandHandler> logger)
        {
            _emailNotificationService = emailNotificationService;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(SendInactivityNotificationCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Sending inactivity notification to User {UserId}", request.UserId);

            var user =  _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found. Skipping notification.", request.UserId);
                return Unit.Value;
            }

            // Construct notification message
            var notificationPayload = new Notification
            {
                UserId = user.Id,
                Title = "We miss you at Fliq!",
                Message = request.Message,
            };

            await _emailNotificationService.SendEmailNotification(notificationPayload);

            _logger.LogInformation("Inactivity notification sent to User {UserId}", request.UserId);
            return Unit.Value;
        }

    }

}
