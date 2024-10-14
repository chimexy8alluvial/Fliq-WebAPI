namespace Fliq.Domain.Entities.Profile
{
    public class WantKids : Record
    {
        public WantKidsType WantKidsType { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum WantKidsType
    {
        Yes,
        No,
        PreferNotToSay
    }
}