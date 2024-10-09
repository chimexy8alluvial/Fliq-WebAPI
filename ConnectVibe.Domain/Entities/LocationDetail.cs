namespace Fliq.Domain.Entities.Profile
{
    public class PlusCode : Record
    {
        public string CompoundCode { get; set; } = default!;
        public string GlobalCode { get; set; } = default!;
    }

    public class AddressComponent : Record
    {
        public string LongName { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class Locationn : Record
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Viewport : Record
    {
        public Locationn Northeast { get; set; } = default!;
        public Locationn Southwest { get; set; } = default!;
    }

    public class Geometry : Record
    {
        public Locationn Location { get; set; } = default!;
        public string LocationType { get; set; } = default!;
        //public Viewport Viewport { get; set; } = default!;
    }

    public class LocationResult : Record
    {
        public List<AddressComponent> AddressComponents { get; set; } = default!;
        public string FormattedAddress { get; set; } = default!;
        public Geometry Geometry { get; set; } = default!;
        public string PlaceId { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class LocationDetail : Record
    {
        public List<LocationResult> Results { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int LocationId { get; set; }
        public Location Location { get; set; } = default!;
    }
}