namespace ConnectVibe.Domain.Entities.Profile
{
    public class WantKids
    {
        public int Id { get; set; }
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