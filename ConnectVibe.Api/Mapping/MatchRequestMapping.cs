using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Common;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class MatchRequestMapping : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateMatchListResult, GetMatchRequestListCommand>().IgnoreNullValues(true);
        }
    }
}
