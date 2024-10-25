using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateAcceptMatchResult
    (
        int MatchInitiatorUserId,
        MatchRequestStatus matchRequestStatus
    );
}
