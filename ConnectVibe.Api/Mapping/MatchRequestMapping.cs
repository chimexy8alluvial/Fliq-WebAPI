using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Contracts.MatchedProfile;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class MatchRequestMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateMatchListResult, MatchedProfileResponse>().Map(dest => dest, src => src.MatchRequestDto);
            config.NewConfig<MatchRequestDto, MatchedProfileResponse>();
            config.NewConfig<CreateMatchProfileResult, MatchedProfileResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).
                Map(dest => dest.Name, src => src.Name).Map(dest => dest.PictureUrl, src => src.PictureUrl).Map(dest => dest.Age, src => src.Age);
            config.NewConfig<CreateAcceptMatchResult, MatchedProfileResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).Map
                (dest => dest.matchRequestStatus, src => src.matchRequestStatus);
            config.NewConfig<RejectMatchResult, MatchedProfileResponse>().Map(dest => dest.MatchInitiatorUserId, src => src.MatchInitiatorUserId).Map
                (dest => dest.matchRequestStatus, src => src.matchRequestStatus);
        }
    }
}
