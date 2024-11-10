

using Fliq.Domain.Entities.Notifications;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface INotificationRepository
    {
        void Add(Notification notification);
        void RegisterDeviceToken(UserDeviceToken userDeviceToken);
    }
}
