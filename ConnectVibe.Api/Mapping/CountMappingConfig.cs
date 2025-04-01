using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class CountMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CountResult, CountResponse>().Map(dest => dest.Count, src => src.Count);
        }
    }
}
