
using MediatR;

namespace Fliq.Application.Notifications.Common
{
    // Base NotificationEvent with common properties
    public abstract record NotificationEvent : INotification
    {
        public int UserId { get; init; }
        public string Title { get; init; } = default!;
        public string Message { get; init; } = default!;
        public string? ImageUrl { get; init; }        // Optional image URL
        public string? ActionUrl { get; init; }       // Optional action URL
        public string? ButtonText { get; init; }      // Optional button text

        protected NotificationEvent(int userId, string title, string message)
        {
            UserId = userId;
            Title = title;
            Message = message;
        }
    }


}
