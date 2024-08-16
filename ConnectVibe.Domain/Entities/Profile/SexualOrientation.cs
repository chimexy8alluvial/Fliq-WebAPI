namespace ConnectVibe.Domain.Entities.Profile
{
    public class SexualOrientation
    {
        public int Id { get; set; }
        public SexualOrientationType SexualOrientationType { get; set; }
        public bool IsVisible { get; set; } = false;
    }

    public enum SexualOrientationType
    {
        Men,
        Women,
        Both
    }
}