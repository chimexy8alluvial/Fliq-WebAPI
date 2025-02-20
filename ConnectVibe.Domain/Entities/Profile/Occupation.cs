namespace Fliq.Domain.Entities.Profile
{
    public class Occupation : Record
    {
        public int UserProfileId { get; set; }
        public string OccupationName { get; set; } = default!;
        public bool IsVisible { get; set; }
    }
}