namespace Fliq.Contracts.Settings
{
    public class FilterDto
    {
        public int Id { get; set; }
        public int LookingFor { get; set; }
        public AgeRangeDto AgeRange { get; set; } = default!;
        public double Distance { get; set; }
        public List<string> Interests { get; set; } = new();
        public List<string> EventPreferences { get; set; } = new();
        public List<int> RacePreferences { get; set; } = new();
        public List<ViceDto> MyVices { get; set; } = new();
        public List<ViceDto> UnacceptableVices { get; set; } = new();
    }

    public class AgeRangeDto
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public AgeRangeDto(int minAge, int maxAge)
        {
            MinAge = minAge;
            MaxAge = maxAge;
        }
    }

    public class ViceDto
    {
        public string Name { get; set; } = default!;
        public bool HaveVice { get; set; }
    }
}