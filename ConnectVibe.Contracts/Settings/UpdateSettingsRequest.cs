namespace Fliq.Contracts.Settings
{
    public record UpdateSettingsRequest
        (
        int Id,
        int ScreenMode,
         bool RelationAvailability,
        bool ShowMusicAndGameStatus,
        string Language,
        List<NotificationPreferenceDto> NotificationPreferences,
        FilterDto Filter,
        int UserId
        );
}