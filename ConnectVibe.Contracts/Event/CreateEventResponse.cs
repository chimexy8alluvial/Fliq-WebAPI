using Fliq.Contracts.Event.ResponseDtos;
using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventResponse(
    int Id,
    string EventType,
    string EventTitle,
    string EventDescription,
    string EventCategory,
    DateTime StartDate,
    DateTime EndDate,
    LocationDto Location,
    int Capacity,
    int MinAge,
    int MaxAge,
    bool SponsoredEvent,
     EventCriteriaDto EventCriteria,
    List<GetTicketResponse> Tickets,
    int UserId,
    GetEventPaymentDetailResponse EventPaymentDetail,
    bool InviteesException,
    SponsoredEventDetailsDto? SponsoredEventDetail
);
}