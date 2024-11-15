using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common.MatchEvents;
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
            string accepterMessage = "You have a new connection";

            await Task.WhenAll(

                // Notify the initiator
                HandleNotificationAsync(
                   notification.MatchInitiatorUserId,
                   notification.Title,
                   notification.Message,
                   notification.InitiatorImageUrl,
                   notification.ActionUrl,
                   notification.ButtonText,
                   cancellationToken
                ),

                // Notify the Accepter
                HandleNotificationAsync(
                    notification.AccepterUserId,
                    notification.Title,
                    accepterMessage,
                    notification.AccepterImageUrl,
                    notification.ActionUrl,
                    notification.ButtonText,
                    cancellationToken
                )
            );
            _logger.LogInfo($"Notification sent for match acceptance to AccepterUserId: {notification.AccepterUserId} and InitiatorUserId: {notification.MatchInitiatorUserId}");
        }

        public async Task Handle(MatchRejectedEvent notification, CancellationToken cancellationToken)
        {
            // Retrieve device tokens for the user
            var deviceTokens = await _notificationRepository.GetDeviceTokensByUserIdAsync(notification.UserId);

            if (deviceTokens.Any())
            {
                // Send notification
                await _firebaseNotificationService.SendNotificationAsync(
                    notification.Title,
                    notification.Message,
                    deviceTokens,
                    notification.UserId
                );
            }

            _logger.LogInfo($"Notification sent for match rejection to UserId: {notification.UserId}");
        }

        public async Task Handle(MatchRequestEvent notification, CancellationToken cancellationToken)
        {
            // Notify the initiator
            await HandleNotificationAsync(
                notification.InitiatorUserId,
                "Match Request Sent",
                "You have initiated a match request.",
                notification.InitiatorImageUrl,
                notification.ActionUrl,
                    notification.ButtonText,
                cancellationToken
            );

            // Notify the accepter
            await HandleNotificationAsync(
                notification.AccepterUserId,
                "New Match Request",
                $"{notification.InitiatorName} has sent you a match request.",
                notification.InitiatorImageUrl,
                notification.ActionUrl,
                    notification.ButtonText,
                cancellationToken
            );
        }


        private async Task HandleNotificationAsync(int userId, string title, string message, string? imageUrl, string? actionUrl, string? buttonText, CancellationToken cancellationToken)
        {
    
            // Retrieve device tokens for the user
            var deviceTokens = await _notificationRepository.GetDeviceTokensByUserIdAsync(userId);

            // Send notification to Firebase if there are tokens available
            if (deviceTokens.Any())
            {
                await _firebaseNotificationService.SendNotificationAsync(title, message, deviceTokens, userId, imageUrl, actionUrl, buttonText);
            }

            _logger.LogInfo($"Notification sent for event to UserId: {userId}");
        }
    }
}