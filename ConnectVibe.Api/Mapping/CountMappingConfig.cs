using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{
<<<<<<<< HEAD:ConnectVibe.Api/Mapping/CountMappingConfig.cs
    public class CountMappingConfig : IRegister
========
    public class DashBoardCountMappingConfig : IRegister
>>>>>>>> development:ConnectVibe.Api/Mapping/DashBoardCountMappingConfig.cs
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CountResult, CountResponse>().Map(dest => dest.Count, src => src.Count);
        }
    }
}
