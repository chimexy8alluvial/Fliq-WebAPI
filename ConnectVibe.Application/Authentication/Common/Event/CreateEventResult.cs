using Fliq.Domain.Entities.Event;
namespace Fliq.Application.Authentication.Common.Event
{
    public record CreateEventResult
    (
        int EventId,
        string Email,
        string EventType,
        List<EventDocument> docss

    );
}
