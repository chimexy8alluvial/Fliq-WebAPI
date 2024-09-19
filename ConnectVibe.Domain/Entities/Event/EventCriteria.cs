using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class EventCriteria
    {
        public int Id { get; set; }
        public Event_Type Event_Type { get; set; }
        public Gender Gender { get; set; }
        public string Race { get; set; } = default!;
    }
}
