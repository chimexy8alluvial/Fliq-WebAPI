namespace ConnectVibe.Domain.Entities.Profile
{
    public class PlusCode
    {
        public int Id { get; set; }
        public string CompoundCode { get; set; } = default!;
        public string GlobalCode { get; set; } = default!;
    }

    public class AddressComponent
    {
        public int Id { get; set; }

        public string LongName { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class Locationn
    {
        public int Id { get; set; }

        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Viewport
    {
        public int Id { get; set; }

        public Locationn Northeast { get; set; } = default!;
        public Locationn Southwest { get; set; } = default!;
    }

    public class Geometry
    {
        public int Id { get; set; }

        public Locationn Location { get; set; } = default!;
        public string LocationType { get; set; } = default!;
        //public Viewport Viewport { get; set; } = default!;
    }

    public class LocationResult
    {
        public int Id { get; set; }

        public List<AddressComponent> AddressComponents { get; set; } = default!;
        public string FormattedAddress { get; set; } = default!;
        public Geometry Geometry { get; set; } = default!;
        public string PlaceId { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class LocationDetail
    {
        public int Id { get; set; }

        
        public List<LocationResult> Results { get; set; } = default!;
        public string Status { get; set; } = default!;
        public int LocationId { get; set; }
        public Location Location { get; set; } = default!;
    }
}