using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Application.Notifications.Common.EventCreatedEvents;
using Fliq.Application.Notifications.Common.MatchEvents;
using MediatR;


namespace Fliq.Application.Notifications.Common
{
    public class NotificationEventHandler : INotificationHandler<MatchAcceptedEvent>,INotificationHandler<MatchRejectedEvent>,
                                            INotificationHandler<MatchRequestEvent>, INotificationHandler<EventCreatedEvent>,
                                            INotificationHandler<EventReviewSubmittedEvent>,  INotificationHandler<TicketPurchasedEvent>


    {
        private readonly INotificationRepository _notificationRepository;
        private readonly IPushNotificationService _firebaseNotificationService;
        private readonly IEmailService _emailService;
        private readonly ILoggerManager _logger;
        public NotificationEventHandler(INotificationRepository notificationRepository, IPushNotificationService firebaseNotificationService, IEmailService emailService, ILoggerManager logger)
        {
            _notificationRepository = notificationRepository;
            _firebaseNotificationService = firebaseNotificationService;
            _emailService = emailService;
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
                notification.OrganizerId,
                notification.Title,
                notification.Message,
                notification.ImageUrl,
                notification.ActionUrl,
                notification.ButtonText,
                cancellationToken
            );
            _logger.LogInfo($"Notification sent to Event Organizer: {notification.OrganizerId} for EventId: {notification.EventId}");

        }

        public async Task Handle(TicketPurchasedEvent notification, CancellationToken cancellationToken)
        {
            // Notify Buyer
            await HandleNotificationAsync(
                notification.BuyerId,
                "Ticket Purchase Successful!",
                $"You have successfully purchased {notification.NumberOfTickets} " +
                $"ticket(s) for the event '{notification.EventTitle}' on {notification.EventDate}.",
                notification.ImageUrl,
                notification.ActionUrl,
                notification.ButtonText,
                cancellationToken
            );

            // Notify Organizer
            await HandleNotificationAsync(
                notification.OrganizerId,
                "New Ticket Purchased",
                $"{notification.BuyerName} purchased {notification.NumberOfTickets} ticket(s) for your event '{notification.EventTitle}'.",
                notification.ImageUrl,
                notification.ActionUrl,
                notification.ButtonText,
                cancellationToken
            );

            // Notify Ticket Assignees from TicketDetails
            if (notification.TicketDetails != null && notification.TicketDetails.Any())
            {
                foreach (var ticket in notification.TicketDetails)
                {
                    if (ticket.UserId.HasValue && ticket.UserId.Value != notification.BuyerId) // Avoid duplicate for buyer
                    {
                        await HandleNotificationAsync(
                            ticket.UserId.Value,
                            "Ticket Assigned",
                            $"You’ve been assigned a {ticket.TicketCategory} ticket for '{notification.EventTitle}' on {notification.EventDate}.",
                            notification.ImageUrl,
                            notification.ActionUrl,
                            notification.ButtonText,
                            cancellationToken
                        );
                    }
                    else if (!string.IsNullOrEmpty(ticket.Email))
                    {
                        await _emailService.SendEmailAsync(
                            ticket.Email,
                            "Ticket Assigned",
                            $"You’ve been assigned a {ticket.TicketCategory} ticket for '{notification.EventTitle}' on {notification.EventDate}."
                        );
                        _logger.LogInfo($"Email sent to {ticket.Email} for ticket assignment.");
                    }
                }
            }

            _logger.LogInfo($"Notifications sent for ticket purchase: BuyerId {notification.BuyerId}, OrganizerId {notification.OrganizerId}.");
        }

        private async Task HandleNotificationAsync(int userId, string title, string message, string? imageUrl, string? actionUrl, string? buttonText, CancellationToken cancellationToken)
        {
            var deviceTokens = await _notificationRepository.GetDeviceTokensByUserIdAsync(userId);

            if (deviceTokens == null || !deviceTokens.Any())
            {
                _logger.LogInfo($"No registered device tokens for UserId: {userId}");
                return;
            }

            _logger.LogInfo($"Device tokens retrieved for UserId: {userId}: {string.Join(", ", deviceTokens)}");
            await _firebaseNotificationService.SendNotificationAsync(title, message, deviceTokens, userId, imageUrl, actionUrl, buttonText);
            _logger.LogInfo($"Notification sent for event to UserId: {userId}");
        }
    }
}