

namespace Fliq.Domain.Entities.DatingEnvironment
{
    public class BlindDateCategory : Record
    {
        public string CategoryName { get; set; } = default!;

        public string? Description { get; set; }

        public ICollection<BlindDate> BlindDates { get; set; } = new List<BlindDate>();
    }
}
