using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Notifications;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly FliqDbContext _dbContext;

        private readonly IDbConnectionFactory _connectionFactory;

        public NotificationRepository(FliqDbContext dbContext, IDbConnectionFactory connectionFactory)
        {
            _dbContext = dbContext;
            _connectionFactory = connectionFactory;
        }

        public void Add(Notification notification)
        {
            if (notification.Id > 0)
            {
                _dbContext.Update(notification);
            }
            else
            {
                _dbContext.Add(notification);
            }
            _dbContext.SaveChanges();
        }

        public void RegisterDeviceToken(UserDeviceToken userDeviceToken)
        {
            if(userDeviceToken.Id > 0)
            {
                _dbContext.Update(userDeviceToken);
            }
            else
            {
                _dbContext.Add(userDeviceToken);
            }
            _dbContext.SaveChanges();
        }
    }
}
