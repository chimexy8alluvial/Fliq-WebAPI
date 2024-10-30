using Fliq.Application.MatchedProfile.Commands.RejectMatch;
using Fliq.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.MatchedProfile.Common
{
    public record RejectMatchResult
    (
        int MatchInitiatorUserId,
        MatchRequestStatus matchRequestStatus
    );
}
