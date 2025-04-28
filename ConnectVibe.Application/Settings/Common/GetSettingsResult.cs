using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;

namespace Fliq.Application.Settings.Common
{
    public record GetSettingsResult(
    int Id,
    string ScreenMode,
    bool RelationAvailability,
    bool ShowMusicAndGameStatus,
    Language Language,
    List<NotificationPreference> NotificationPreferences,
    Filter Filter,
    string Name,
    string Email,
    int UserId
        );
}