namespace Fliq.Domain.Entities.DatingEnvironment.BlindDates
{
    public class BlindDateCategory : Record
    {
        public string CategoryName { get; set; } = default!;

        public string Description { get; set; } = default!;

        public ICollection<BlindDate> BlindDates { get; set; } = new List<BlindDate>();
    }
}
