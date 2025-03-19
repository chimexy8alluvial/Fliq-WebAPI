
namespace Fliq.Contracts.Settings
{
    public record NotificationPreferenceDto
        (
            int Id,
    string Context,
    bool PushNotification,
    bool InAppNotification
        );
}