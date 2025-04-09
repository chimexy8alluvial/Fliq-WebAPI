using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class EventCriteria : Record
    {
        public Event_Type EventType { get; set; }
        public GenderType? Gender { get; set; }
        public string Race { get; set; } = default!;
    }
}