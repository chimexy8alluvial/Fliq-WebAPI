namespace Fliq.Application.Common.Models
{
    public class PlusCode
    {
        public string CompoundCode { get; set; } = default!;
        public string GlobalCode { get; set; } = default!;
    }

    public class AddressComponent
    {
        public string LongName { get; set; } = default!;
        public string ShortName { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class Locationn
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Viewport
    {
        public Locationn Northeast { get; set; } = default!;
        public Locationn Southwest { get; set; } = default!;
    }

    public class Geometry
    {
        public Locationn Location { get; set; } = default!;
        public string LocationType { get; set; } = default!;
        public Viewport Viewport { get; set; } = default!;
    }

    public class Result
    {
        public List<AddressComponent> AddressComponents { get; set; } = default!;
        public string FormattedAddress { get; set; } = default!;
        public Geometry Geometry { get; set; } = default!;
        public string PlaceId { get; set; } = default!;
        public PlusCode PlusCode { get; set; } = default!;
        public List<string> Types { get; set; } = default!;
    }

    public class LocationQueryResponse
    {
        public PlusCode PlusCode { get; set; } = default!;
        public List<Result> Results { get; set; } = default!;
        public string Status { get; set; } = default!;
        public string? CurrencyCode { get; set; }
    }
}