namespace Fliq.Domain.Entities.Profile
{
    public class HaveKids : Record
    {
        public int UserProfileId { get; set; }
        public HaveKidsType HaveKidsType { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum HaveKidsType
    {
        Yes,
        No,
        PreferNotToSay
    }
}