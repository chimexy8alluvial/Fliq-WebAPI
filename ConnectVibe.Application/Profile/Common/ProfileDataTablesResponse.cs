using Fliq.Domain.Entities.Profile;

namespace Fliq.Application.Profile.Common
{
    public class ProfileDataTablesResponse
    {
        public List<Occupation> Occupations { get; set; } = new List<Occupation>();
        public List<Religion> Religions { get; set; } = new List<Religion>();
        public List<Ethnicity> Ethnicities { get; set; } = new List<Ethnicity>();
        public List<EducationStatus> EducationStatuses { get; set; } = new List<EducationStatus>();
        public List<Gender> Genders { get; set; } = new List<Gender>();
        public List<HaveKids> HaveKids { get; set; } = new List<HaveKids>();
        public List<WantKids> WantKids { get; set; } = new List<WantKids>();
    }
}
