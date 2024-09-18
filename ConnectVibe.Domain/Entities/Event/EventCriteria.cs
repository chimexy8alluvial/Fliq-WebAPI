using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class EventCriteria
    {
        public int Id { get; set; }
        public Event_Type EventType { get; set; }
        public Gender Gender { get; set; }
        public List<string> Race { get; set; } = default!;
    }
}
