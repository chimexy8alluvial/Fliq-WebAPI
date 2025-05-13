using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;


namespace Fliq.Infrastructure.Services.NotificationServices.Firebase
{
    public class FireBaseNotificationService : IPushNotificationService
    {
        private readonly ILoggerManager _logger;
        private readonly INotificationRepository _notificationRepository;
        private readonly IFirebaseMessagingWrapper _firebaseWrapper;
        private readonly bool _isInitialized;

        public FireBaseNotificationService(
            ILoggerManager logger,
            INotificationRepository notificationRepository,
            IFirebaseMessagingWrapper firebaseWrapper)
        {
            _logger = logger;
            _notificationRepository = notificationRepository;
            _firebaseWrapper = firebaseWrapper;

            try
            {
                if (FirebaseApp.DefaultInstance == null)
                {
                    var credentialsJson = Environment.GetEnvironmentVariable("FIREBASE_CREDENTIALS");
                    if (string.IsNullOrEmpty(credentialsJson))
                    {
                        _logger.LogWarn("FIREBASE_CREDENTIALS environment variable not set. Notifications disabled.");
                    }
                    else
                    {
                        _logger.LogInfo("Initializing Firebase with credentials from environment variable");
                        FirebaseApp.Create(new AppOptions
                        {
                            Credential = Google.Apis.Auth.OAuth2.GoogleCredential.FromJson(credentialsJson)
                        });
                        _isInitialized = true;
                    }
                }
                else
                {
                    _isInitialized = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to initialize Firebase: {ex.Message}");
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
            if (!_isInitialized)
            {
                _logger.LogWarn($"Firebase not initialized. Skipping notification for UserId {userId}");
                return;
            }

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

            _notificationRepository.Add(notificationRecord);

            var notification = new Notification
            {
                Title = title,
                Body = message,
                ImageUrl = imageUrl
            };

            var messagePayload = new MulticastMessage
            {
                Tokens = deviceTokens,
                Notification = notification,
                Data = new Dictionary<string, string>
                {
                    { "actionUrl", actionUrl ?? "" },
                    { "buttonText", buttonText ?? "" }
                }
            };

            try
            {
                var response = await _firebaseWrapper.SendEachForMulticastAsync(messagePayload);
                _logger.LogInfo($"Sent {response.SuccessCount} notifications; {response.FailureCount} failed for UserId {userId}");
            }
            catch (FirebaseMessagingException fex)
            {
                _logger.LogError($"Firebase messaging error for UserId {userId}: {fex.ErrorCode}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending notification to UserId {userId}: {ex.Message}");
            }
        }
    }
}