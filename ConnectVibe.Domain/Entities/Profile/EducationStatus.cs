namespace Fliq.Domain.Entities.Profile
{
    public class EducationStatus : Record
    {
        public EducationLevel EducationLevel { get; set; }
        public bool IsVisible { get; set; }
    }

    public enum EducationLevel
    {
        HighSchool,
        AssociateDegree,
        BachelorDegree,
        MasterDegree,
        Doctorate,
        Other,
        PreferNotToSay
    }
}