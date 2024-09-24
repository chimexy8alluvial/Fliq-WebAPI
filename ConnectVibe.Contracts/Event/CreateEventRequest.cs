using ConnectVibe.Contracts.Profile;
using Fliq.Contracts.Event.Enums;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        EventTypeDtoEnum Type,
        string Title,
        string Description,
        DateTime StartDate,
        DateTime EndDate,
        string Location,
        double Capacity,
        int UserId,
        List<EventMediaDto> MediaDocuments,
        string StartAge,
        string EndAge,
        string Category,
        bool IsSponsored,
     //   SponsoredEventDetailsDto SponsoredDetail,
       // EventCriteriaDto Criteria,
        List<TicketTypeDto> TicketType
    );
}
