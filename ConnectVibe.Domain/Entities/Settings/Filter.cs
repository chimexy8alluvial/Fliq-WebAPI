using Fliq.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.Settings
{
    public class Filter : Record
    {
        public LookingFor LookingFor { get; set; }
        public AgeRange AgeRange { get; set; } = default!;
        public double Distance { get; set; }
        public List<string> Interests { get; set; } = new();
        public List<string> EventPreferences { get; set; } = new();
        public List<EthnicityType> RacePreferences { get; set; } = new();
        public List<Vice> MyVices { get; set; } = new();
        public List<Vice> UnacceptableVices { get; set; } = new();
        public Setting Setting { get; set; }
        public int SettingId { get; set; }
    }

    public enum LookingFor
    {
        Friendship,
        Relationship,
        Both
    }

    public class AgeRange : Record
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public AgeRange(int minAge, int maxAge)
        {
            MinAge = minAge;
            MaxAge = maxAge;
        }
    }

    public class Vice : Record
    {
        public string Name { get; set; } = default!;
        public bool HaveVice { get; set; }
    }
}