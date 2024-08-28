namespace ConnectVibe.Domain.Entities.Profile
{
    public class Gender
    {
        public int Id { get; set; }
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