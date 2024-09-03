using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Event
{
    public  class CreateEvent
    {
        public int EventId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string EventType { get; set; } = default!;
        public List<EventDocument> document { get; set; } = default!;
    }

    public class Document
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public bool IsVarified { get; set; }
    }

}
