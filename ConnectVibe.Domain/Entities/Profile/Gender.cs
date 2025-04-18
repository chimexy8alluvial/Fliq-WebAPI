using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Domain.Entities.Profile
{
    public class Gender : Record
    {
        public GenderType GenderType { get; set; } = default!;
    }

}