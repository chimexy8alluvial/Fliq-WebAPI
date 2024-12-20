namespace Fliq.Contracts.Settings
{
    public class FilterDto
    {
        public int Id { get; set; }
        public int LookingFor { get; set; }
        public int MinAge { get; set; }
        public int MaxAge { get; set; }
        public double Distance { get; set; }
        public List<string> Interests { get; set; } = new();
        public List<string> EventPreferences { get; set; } = new();
        public List<int> RacePreferences { get; set; } = new();
        public List<ViceDto> MyVices { get; set; } = new();
        public List<ViceDto> UnacceptableVices { get; set; } = new();
    }

    public class ViceDto
    {
        public string Name { get; set; } = default!;
        public bool HaveVice { get; set; }
    }
}