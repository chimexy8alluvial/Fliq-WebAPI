﻿using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Event.Common
{
    public record CreateEventTicketResult(
        List<EventTicket> EventTickets
        );
}