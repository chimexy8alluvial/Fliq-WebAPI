

using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Event
{
    public class DatingOptions : Record
    {
        public int id { get; set; } 
        public DatingType DatingType { get; set; }

    }
}
