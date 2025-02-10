

using Fliq.Domain.Entities.Notifications;

namespace Fliq.Application.Common.Interfaces.Services.NotificationServices
{
    public interface IEmailNotificationService
    {
        Task SendEmailNotification(Notification notification);
    }
}
