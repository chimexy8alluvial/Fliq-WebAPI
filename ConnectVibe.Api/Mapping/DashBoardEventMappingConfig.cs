using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.DashBoard;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class DashBoardEventMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<GetEventsResult, GetEventsResponse>().Map(dest => dest.EventTitle, src => src.EventTitle)
                                                                .Map(dest => dest.CreatedBy, src => src.CreatedBy)
                                                                .Map(dest => dest.Status, src => src.Status)
                                                                .Map(dest => dest.Attendees, src => src.Attendees)
                                                                .Map(dest => dest.Type, src => src.Type)
                                                                .Map(dest => dest.CreatedOn, src => src.CreatedOn);
        }
    }
}
