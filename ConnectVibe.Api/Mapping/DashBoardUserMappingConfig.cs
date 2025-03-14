using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class DashBoardUserMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<GetUsersResult, GetUsersResponse>().Map(dest => dest.DisplayName, src => src.DisplayName)
                                                                .Map(dest => dest.Email, src => src.Email)
                                                                .Map(dest => dest.SubscriptionType, src => src.SubscriptionType)
                                                                .Map(dest => dest.DateJoined, src => src.DateJoined)
                                                                .Map(dest => dest.LastOnline, src => src.LastOnline);
        }
    }
}
