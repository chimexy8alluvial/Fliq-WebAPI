﻿using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Entities.Notifications;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly FliqDbContext _dbContext;


        public NotificationRepository(FliqDbContext dbContext)
        {
            _dbContext = dbContext;
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
            if (userDeviceToken.Id > 0)
            {
                _dbContext.Update(userDeviceToken);
            }
            else
            {
                _dbContext.Add(userDeviceToken);
            }
            _dbContext.SaveChanges();
        }

        public async Task<List<string>> GetDeviceTokensByUserIdAsync(int userId)
        {
            return await _dbContext.UserDeviceTokens
                .Where(udt => udt.UserId == userId)
                .Select(udt => udt.DeviceToken)
                .ToListAsync();
        }

    }
}
