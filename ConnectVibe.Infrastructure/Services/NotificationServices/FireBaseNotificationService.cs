using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;


namespace Fliq.Infrastructure.Services.NotificationServices
{
    public class FireBaseNotificationService : INotificationService
    {
        private readonly ILoggerManager _logger;
        private readonly INotificationRepository _notificationRepository;
        public FireBaseNotificationService(ILoggerManager logger, INotificationRepository notificationRepository)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;

            // Initialize Firebase if not already initialized
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions
                {                                                                 // update this path to correct directory where the private keys file will be stored
                    Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromFile("path/to/your/firebase/credentials.json") 
                });
            }
        }


        public async Task SendNotificationAsync(
            string title,
            string message,
            List<string> deviceTokens,
            int userId,
            string? imageUrl = null,
            string? actionUrl = null,
            string? buttonText = null)
        {
            // Create a new notification record
            var notificationRecord = new Domain.Entities.Notifications.Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                ActionUrl = actionUrl,
                ButtonText = buttonText,
                IsRead = false,
                DateCreated = DateTime.Now
            };

            // Save the notification record
            _notificationRepository.Add(notificationRecord);

            // Build the Firebase notification payload
            var notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = message,
                ImageUrl = imageUrl
            };

            var messagePayload = new MulticastMessage
            {
                Tokens = deviceTokens,
                Notification = notification,
                Data = new Dictionary<string, string>  // Additional data for action URL and button text
                {
                    { "actionUrl", actionUrl ?? "" },
                    { "buttonText", buttonText ?? "" }
                }
            };

            // Send notification via Firebase

            try
            {
                var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(messagePayload);
                _logger.LogInfo($"Sent {response.SuccessCount} notifications; {response.FailureCount} failed.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to UserId {userId}: {ex.Message}");
            }

        }

    }
}
