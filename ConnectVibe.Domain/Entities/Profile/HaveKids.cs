namespace Fliq.Domain.Entities.Profile
{
    public class HaveKids : Record
    {
        public string HaveKidsType { get; set; } = default!;
    }

    public enum HaveKidsType
    {
        Yes,
        No,
        PreferNotToSay
    }
}