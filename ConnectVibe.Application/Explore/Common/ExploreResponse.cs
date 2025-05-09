﻿using Fliq.Application.Common.Pagination;
using Fliq.Contracts.Profile;

namespace Fliq.Application.Explore.Common
{
    public record ExploreResponse(PaginationResponse<ExploreProfileResponse> UserProfiles);
}
