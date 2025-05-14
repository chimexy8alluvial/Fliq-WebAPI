using Fliq.Application.Common.Pagination;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using Fliq.Contracts.Profile;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class ExploreMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<ExploreResult, ExploreResponse>()
            .Map(dest => dest.UserProfiles, src =>
                new PaginationResponse<ProfileResponse>(
                    src.UserProfiles.Data.Select(userProfile => userProfile.Adapt<ProfileResponse>()).ToList(),
                    src.UserProfiles.TotalCount,
                    src.UserProfiles.PageNumber,
                    src.UserProfiles.PageSize
                ));

            config.NewConfig<ExploreQuery, ExploreRequest>();

            config.NewConfig<ExploreEventsQuery, ExploreEventsRequest>();

            config.NewConfig<ExploreEventsResult, ExploreEventsResponse>();
        }
    }
}
