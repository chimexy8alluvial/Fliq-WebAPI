namespace ConnectVibe.Domain.Entities.Profile
{
    public class Location
    {
        public int Id { get; set; }
        public string Address { get; set; } = default!;
        public string PostCode { get; set; } = default!;
        public string Country { get; set; } = default!;
        public bool IsVisible { get; set; }
    }
}