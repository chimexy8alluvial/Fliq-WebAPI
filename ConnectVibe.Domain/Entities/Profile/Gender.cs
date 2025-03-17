namespace Fliq.Domain.Entities.Profile
{
    public class Gender : Record
    {
        public int UserProfileId { get; set; }
        public GenderType GenderType { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum GenderType
    {
        Male,
        Female,
        Others
    }
}