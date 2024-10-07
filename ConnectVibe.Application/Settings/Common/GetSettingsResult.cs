using Fliq.Domain.Entities.Settings;

namespace Fliq.Application.Settings.Common
{
    public record GetSettingsResult(
    int Id,
    ScreenMode ScreenMode,
    bool RelationAvailability,
    bool ShowMusicAndGameStatus,
    string Language,
    List<NotificationPreference> NotificationPreferences,
    Filter Filter,
    string Name,
    string Email,
    int UserId
        );
}