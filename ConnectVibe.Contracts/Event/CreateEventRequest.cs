﻿using Fliq.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        int EventType,
    string EventTitle,
    string EventDescription,
    int EventCategory,
    DateTime StartDate,
    DateTime EndDate,
    LocationDto Location,
    int Capacity,
   int MinAge,
  int Maxge,
    bool SponsoredEvent,
    EventCriteriaDto EventCriteria,
    List<TicketDto>? Tickets,
    int UserId,
    EventPaymentDetailDto? EventPaymentDetail,
    bool InviteesException,
    List<EventMediaDto> MediaDocuments,
    SponsoredEventDetailsDto? SponsoredEventDetail
    );
}