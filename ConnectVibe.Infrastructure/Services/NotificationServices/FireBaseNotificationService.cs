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

        public async Task SendNotificationAsync(string title, string message, List<string> deviceTokens, int userId)
        {
            // Create a new notification record
            var notificationRecord = new Domain.Entities.Notifications.Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                IsRead = false,
                DateCreated = DateTime.Now
            };

            // Use the notification repository to save the notification
            _notificationRepository.Add(notificationRecord);

            // Build the Firebase notification payload
            var notification = new FirebaseAdmin.Messaging.Notification
            {
                Title = title,
                Body = message
            };

            var messagePayload = new MulticastMessage
            {
                Tokens = deviceTokens,
                Notification = notification
            };

            // Send notification via Firebase
            var response = await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(messagePayload);

            _logger.LogInfo($"Sent {response.SuccessCount} notifications; {response.FailureCount} failed.");
        }
    }
}
