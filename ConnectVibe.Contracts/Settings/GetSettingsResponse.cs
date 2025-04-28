namespace Fliq.Contracts.Settings
{
    public record GetSettingsResponse
        (
         int Id,
    string ScreenMode,
    bool RelationAvailability,
    bool ShowMusicAndGameStatus,
    int Language,
    List<NotificationPreferenceDto> NotificationPreferences,
    string Name,
    string Email,
    int UserId
        );
}