
namespace Fliq.Contracts.Settings
{
    public record GetSettingsResponse
        (
         int Id,
    int ScreenMode,
    bool RelationAvailability,
    bool ShowMusicAndGameStatus,
    string Language,
    List<NotificationPreferenceDto> NotificationPreferences,
    string Name,
    string Email,
    int UserId
        );
}