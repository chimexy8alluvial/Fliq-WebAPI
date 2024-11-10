﻿

namespace Fliq.Application.Common.Interfaces.Services.NotificationServices
{
    public interface INotificationService
    {
        Task SendNotificationAsync(string title, string message, List<string> deviceTokens, int userId);
    }
}
