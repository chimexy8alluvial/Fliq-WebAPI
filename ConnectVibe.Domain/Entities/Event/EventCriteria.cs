using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class EventCriteria : Record
    {
        public EventCategory EventType { get; set; }
        public Gender? Gender { get; set; }
    }
}