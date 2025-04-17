namespace Fliq.Contracts.Explore
{
    public class LocationDto
    {
        public double Lat { get; init; }
        public double Lng { get; init; }
        public bool IsVisible { get; init; }
        public LocationDetailDto? LocationDetail { get; init; }
    }

    public class LocationDetailDto
    {
        public string Status { get; init; } = string.Empty;
        public List<LocationResultDto>? Results { get; init; }
    }

    public class LocationResultDto
    {
        public string FormattedAddress { get; init; } = string.Empty;
        public GeometryDto Geometry { get; init; } = default!;
        public List<object>? AddressComponents { get; init; }
        public List<string>? Types { get; init; }
        public string PlaceId { get; init; } = string.Empty;
    }

    public class GeometryDto
    {
        public LocationnDto Location { get; init; } = default!;
        public string LocationType { get; init; } = string.Empty;
    }

    public class LocationnDto
    {
        public double Lat { get; init; }
        public double Lng { get; init; }
    }
}
