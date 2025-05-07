using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Profile
{
    public class Gender : Record
    {
        public  string GenderType { get; set; } = default!;
    }

}