namespace Fliq.Domain.Entities.Profile
{
    public class HaveKids : Record
    {
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