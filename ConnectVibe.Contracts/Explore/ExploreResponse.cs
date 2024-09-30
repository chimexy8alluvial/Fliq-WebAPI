﻿using Fliq.Contracts.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Contracts.Explore
{
    public record ExploreResponse(IEnumerable<ProfileResponse> UserProfiles,
        int TotalCount,
        int PageNumber,
        int PageSize);
}
