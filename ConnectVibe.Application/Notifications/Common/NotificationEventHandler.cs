using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Application.Notifications.Common.MatchEvents;
using MediatR;


namespace Fliq.Application.Notifications.Common
{
    public class NotificationEventHandler : INotificationHandler<MatchAcceptedEvent>,INotificationHandler<MatchRejectedEvent>,
                                            INotificationHandler<MatchRequestEvent>, INotificationHandler<EventCreatedEvent>, INotificationHandler<EventReviewSubmittedEvent>
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
                null,
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

        public async Task Handle(EventCreatedEvent notification, CancellationToken cancellationToken)
        {
            // Notify the organizer
            await HandleNotificationAsync(
                notification.OrganizerId,
                notification.Title,
                notification.Message,
                notification.OrganizerImageUrl,
                notification.ActionUrl,
                notification.ButtonText,
                cancellationToken
            );
            _logger.LogInfo($"Notification sent to Event Organizer: {notification.OrganizerId} for EventId: {notification.EventId}");

            // Notify each invitee
            if (notification.InviteeIds != null && notification.InviteeIds.Any())
            {
                foreach (var inviteeId in notification.InviteeIds)
                {
                    if (inviteeId == notification.OrganizerId)
                    {
                        _logger.LogInfo($"Skipping notification for invitee {inviteeId} as they are the organizer.");
                        continue;
                    }

                    await HandleNotificationAsync(
                        inviteeId,
                        notification.IsUpdated ? "Event Updated!" : "You're Invited!",
                        notification.IsUpdated
                            ? $"{notification.OrganizerName} has updated the event '{notification.Title}'."
                            : $"{notification.OrganizerName} has invited you to the event '{notification.Title}'.",
                        notification.OrganizerImageUrl,
                        notification.ActionUrl,
                        notification.IsUpdated ? "View Updated Event" : "View Invitation",
                        cancellationToken
                    );

                    _logger.LogInfo($"Notification sent to Invitee: {inviteeId} for EventId: {notification.EventId}");
                }
            }
        }

        public async Task Handle(EventReviewSubmittedEvent notification, CancellationToken cancellationToken)
        {
            // Notify the organizer
            await HandleNotificationAsync(
                notification.UserId,
                notification.Title,
                notification.Message,
                notification.ImageUrl,
                notification.ActionUrl,
                notification.ButtonText,
                cancellationToken
            );
            _logger.LogInfo($"Notification sent to Event Organizer: {notification.OrganizerId} for EventId: {notification.EventId}");

        }

        private async Task HandleNotificationAsync(int userId, string title, string message, string? imageUrl, string? actionUrl, string? buttonText, CancellationToken cancellationToken)
        {
    
            // Retrieve device tokens for the user
            var deviceTokens = await _notificationRepository.GetDeviceTokensByUserIdAsync(userId);
            _logger.LogInfo($"Device tokens retrieved for UserId: {userId}: {string.Join(", ", deviceTokens)}");


            // Send notification to Firebase if there are tokens available
            if (deviceTokens.Count > 0)
            {
                await _firebaseNotificationService.SendNotificationAsync(title, message, deviceTokens, userId, imageUrl, actionUrl, buttonText);
                _logger.LogInfo($"Notification sent for event to UserId: {userId}");
            }

            else
            {
                _logger.LogInfo($"No registered device tokens for UserId: {userId}");
            }
        }
    }
}