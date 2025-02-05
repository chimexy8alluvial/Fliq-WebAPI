

using Fliq.Domain.Entities.Notifications;

namespace Fliq.Application.Common.Interfaces.Services.NotificationServices
{
    public interface IPushNotificationService
    {
        Task SendNotificationAsync(
            string title,
            string message,
            List<string> deviceTokens,
            int userId,
            string? imageUrl = null,
            string? actionUrl = null,
            string? buttonText = null);
    }
}
