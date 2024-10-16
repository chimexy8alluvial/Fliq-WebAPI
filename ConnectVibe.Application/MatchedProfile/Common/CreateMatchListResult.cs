using ConnectVibe.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.MatchedProfile.Common
{
    public record CreateMatchListResult
    (
        string UserName,
        int Age,
        ProfilePhoto Photos,
        DateTime RequestDate
    );
}
