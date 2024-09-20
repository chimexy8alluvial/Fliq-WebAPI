using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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