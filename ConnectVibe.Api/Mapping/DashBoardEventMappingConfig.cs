using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
                                                                .Map(dest => dest.EventCategory, src => src.EventCategory)
                                                                .Map(dest => dest.CreatedOn, src => src.CreatedOn);
        }
    }
}
