using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class UserCountMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<UserCountResult, UserCountResponse>().Map(dest => dest.Count, src => src.Count);
        }
    }
}
