using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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