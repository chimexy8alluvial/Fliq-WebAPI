

using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.NotificationServices;
using Fliq.Domain.Entities.Notifications;

namespace Fliq.Infrastructure.Services.NotificationServices.Email
{
    public class EmailNotificationService : IEmailNotificationService
    {
        private readonly INotificationRepository _notificationRepository;
        public EmailNotificationService(ILoggerManager logger, INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }



        public async Task SendEmailNotification(Notification notification)
        {
            await Task.CompletedTask;

            _notificationRepository.Add(notification);

            //integrate Sending to email service
        }
    }
}
