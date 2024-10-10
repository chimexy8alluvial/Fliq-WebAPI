namespace Fliq.Domain.Entities.Profile
{
    public class SexualOrientation : Record
    {
        public SexualOrientationType SexualOrientationType { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum SexualOrientationType
    {
        Men,
        Women,
        Both
    }
}