using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common.MatchEvents;
using Fliq.Domain.Entities.Notifications;
using MediatR;


namespace Fliq.Application.Notifications.Common
{
    public class NotificationEventHandler : INotificationHandler<MatchAcceptedEvent>
    {
        private readonly INotificationRepository _notificationRepository;
        private readonly INotificationService _firebaseNotificationService;
        private readonly ILoggerManager _logger;
        public NotificationEventHandler(INotificationRepository notificationRepository, INotificationService firebaseNotificationService, ILoggerManager logger)
        {
            _notificationRepository = notificationRepository;
            _firebaseNotificationService = firebaseNotificationService;
            _logger = logger;
        }



        public async Task Handle(MatchAcceptedEvent notification, CancellationToken cancellationToken)
        {
            await HandleNotificationAsync(
                notification.MatchInitiatorUserId,
                notification.Title,
                notification.Message,
                cancellationToken
            );
        }

        private async Task HandleNotificationAsync(int userId, string title, string message, CancellationToken cancellationToken)
        {
            // Create and save notification to the database
            var dbNotification = new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                DateCreated = DateTime.Now,
            };
            _notificationRepository.Add(dbNotification);

            // Retrieve device tokens for the user
            var deviceTokens = await _notificationRepository.GetDeviceTokensByUserIdAsync(userId);

            // Send notification to Firebase if there are tokens available
            if (deviceTokens.Any())
            {
                await _firebaseNotificationService.SendNotificationAsync(title, message, deviceTokens, userId);
            }

            _logger.LogInfo($"Notification sent for event to UserId: {userId}");
        }
    }
}