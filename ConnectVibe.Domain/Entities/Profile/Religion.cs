namespace Fliq.Domain.Entities.Profile
{
    public class Religion : Record
    {
        public int UserProfileId { get; set; }
        public ReligionType ReligionType { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum ReligionType
    {
        Christianity,
        Islam,
        Others
    }
}