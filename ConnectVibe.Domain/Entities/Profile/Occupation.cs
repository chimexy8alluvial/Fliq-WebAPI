namespace Fliq.Domain.Entities.Profile
{
    public class Occupation : Record
    {
        public string OccupationName { get; set; } = default!;
        public bool IsVisible { get; set; }
    }
}