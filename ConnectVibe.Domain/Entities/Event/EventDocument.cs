using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Event
{
    public class EventDocument
    {
        public int Id { get; set; }
        public string DocumentUrl { get; set; } = default!;
        public string Title { get; set; } = default!;
    }
}
