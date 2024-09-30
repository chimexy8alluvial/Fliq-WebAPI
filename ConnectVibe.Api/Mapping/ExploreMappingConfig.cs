using Fliq.Application.Explore.Common;
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
                .Map(dest => dest.TotalCount, src => src.UserProfiles.TotalCount)
                .Map(dest => dest.PageNumber, src => src.UserProfiles.PageNumber)
                .Map(dest => dest.PageSize, src => src.UserProfiles.PageSize);

        }
    }
}
