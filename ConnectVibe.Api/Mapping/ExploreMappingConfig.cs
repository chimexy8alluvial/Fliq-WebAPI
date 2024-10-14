using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Common.Services;
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
                    src.UserProfiles.Adapt<IEnumerable<ProfileResponse>>())
                .Map(dest => dest.UserProfiles.TotalCount, src => src.UserProfiles.TotalCount)
                .Map(dest => dest.UserProfiles.PageNumber, src => src.UserProfiles.PageNumber)
                .Map(dest => dest.UserProfiles.PageSize, src => src.UserProfiles.PageSize);

            config.NewConfig<ExploreQuery, ExploreRequest>();

            config.NewConfig<ExploreEventsQuery, ExploreEventsRequest>();

            config.NewConfig<ExploreEventsResult, ExploreEventsResponse>();
        }
    }
}
