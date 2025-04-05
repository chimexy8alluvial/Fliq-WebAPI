namespace Fliq.Domain.Entities.Profile
{
    public class WantKids : Record
    {
        public string WantKidsType { get; set; } = default!;
    }

    public enum WantKidsType
    {
        Yes,
        No,
        PreferNotToSay
    }
}