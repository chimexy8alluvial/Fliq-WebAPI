using ConnectVibe.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Authentication.Common.Profile
{
    public record CreateProfileResult(
        UserProfile Profile
        );
}