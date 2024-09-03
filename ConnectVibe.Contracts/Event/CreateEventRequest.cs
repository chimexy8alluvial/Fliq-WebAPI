using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectVibe.Contracts.Profile;
using Microsoft.AspNetCore.Http;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        int EventId,
        string EventType,
        string Email,
        List<EventDocumentDto> Docs
    );
}
