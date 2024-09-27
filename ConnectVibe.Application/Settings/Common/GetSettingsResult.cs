using Fliq.Domain.Entities.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Settings.Common
{
    public record GetSettingsResult(
    int Id,
    ScreenMode ScreenMode,
    bool RelationAvailability,
    bool ShowMusicAndGameStatus,
    string Language,
    List<NotificationPreference> NotificationPreferences,
    string Name,
    string Email,
    int UserId
        );
}