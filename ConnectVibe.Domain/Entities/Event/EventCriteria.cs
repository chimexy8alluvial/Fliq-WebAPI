using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Event
{
    public class EventCriteria
    {
        public int Id { get; set; }
        public Event_Type EventType { get; set; }
        public Gender Gender { get; set; }
        public List<string> Race { get; set; } = default!;
    }

    public enum Event_Type
    {
        Paid,
        Free,
        FreeRequiresApproval,
        RequiresApproval
    }

    public enum Gender
    {
        Male,
        Female
    };
}
