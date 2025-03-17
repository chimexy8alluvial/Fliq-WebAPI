using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class MatchRequestMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateMatchListResult, MatchRequestResponse>().Map(dest => dest, src => src.MatchRequestDto);
            config.NewConfig<MatchRequestDto, MatchRequestResponse>();
            config.NewConfig<CreateMatchRequestResult, MatchRequestResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).
                Map(dest => dest.Name, src => src.Name).Map(dest => dest.PictureUrl, src => src.PictureUrl).Map(dest => dest.Age, src => src.Age).Map(dest => dest.MatchRequestStatus, src => src.MatchRequestStatus);
            config.NewConfig<CreateAcceptMatchResult, MatchRequestResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).Map
                (dest => dest.MatchRequestStatus, src => src.matchRequestStatus);
            config.NewConfig<RejectMatchResult, MatchRequestResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).Map
                (dest => dest.MatchRequestStatus, src => src.matchRequestStatus);
        }
    }
}
