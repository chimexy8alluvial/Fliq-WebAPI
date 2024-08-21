namespace ConnectVibe.Domain.Entities.Profile
{
    public class HaveKids
    {
        public int Id { get; set; }
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