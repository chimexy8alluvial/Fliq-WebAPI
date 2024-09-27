namespace Fliq.Domain.Entities.Profile
{
    public class Location
    {
        public int Id { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool IsVisible { get; set; }
        public LocationDetail LocationDetail { get; set; } = default!;
    }
}