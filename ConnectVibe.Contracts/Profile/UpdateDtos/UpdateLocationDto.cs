namespace Fliq.Contracts.Profile.UpdateDtos
{
    public class UpdateLocationDto
    {
        public int Id { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public bool? IsVisible { get; set; }
    }

    public class ReadLocationDto
    {
        public int Id { get; set; }
        public double? Lat { get; set; }
        public double? Lng { get; set; }
        public bool? IsVisible { get; set; }
    }
}