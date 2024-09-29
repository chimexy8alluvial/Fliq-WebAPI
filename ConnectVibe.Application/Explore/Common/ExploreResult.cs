using Fliq.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.Explore.Common
{
    public record ExploreResult(
        IEnumerable<UserProfile> UserProfiles
        /*,IEnumerable<UserProfile> FriendshipProfiles,
        IEnumerable<Event>? Events = null*/);
    
}
