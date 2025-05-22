using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fliq.Domain.Entities.Profile
{
    public class UserProfile : Record
    {
        public int GenderId { get; set; }
        public int? HaveKidsId { get; set; }
        public int? WantKidsId { get; set; }
        public int? SexualOrientationId { get; set; }
        public bool IsSexualOrientationVisible { get; set; }
        public int? EducationStatusId { get; set; }
        public bool IsEducationStatusVisible { get; set; }
        public int? EthnicityId { get; set; }
        public bool IsEthnicityVisible { get; set; }
        public int? OccupationId { get; set; }
        public bool IsOccupationVisible { get; set; }
        public int? ReligionId { get; set; }
        public bool IsReligionVisible { get; set; }
        

        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
        public string? ProfileDescription { get; set; }  //Nullable
        public SexualOrientation? SexualOrientation { get; set; }  //Nullable
        public Religion? Religion { get; set; }
        public Ethnicity? Ethnicity { get; set; }
        public Occupation? Occupation { get; set; }
        public EducationStatus? EducationStatus { get; set; }
        public HaveKids HaveKids { get; set; } = default!;
        public WantKids WantKids { get; set; } = default!;
        public Location? Location { get; set; } = default!;
        public BusinessIdentificationDocument? BusinessIdentificationDocument { get; set; }
        public bool AllowNotifications { get; set; }
        public List<ProfilePhoto>? Photos { get; set; } = new();

        [Column("Passions")]         //JSON string of Passions in the database
        public string PassionsJson { get; set; } = "[]";

        public List<string> Passions
        {
            get => JsonConvert.DeserializeObject<List<string>>(PassionsJson) ?? new List<string>();
            set => PassionsJson = JsonConvert.SerializeObject(value);
        }

        [Column("ProfileTypes")]        // This property will be mapped to the database as a JSON string
        public string ProfileTypeJson { get; set; } = "[]"; // Initialize with an empty JSON array

        [NotMapped]         // Prevent this from being mapped to the database
        public List<ProfileType> ProfileTypes
        {
            get => JsonConvert.DeserializeObject<List<ProfileType>>(ProfileTypeJson) ?? new List<ProfileType>();
            set => ProfileTypeJson = JsonConvert.SerializeObject(value);
        }

        public List<string> CompletedSections { get; set; } = new();

        public int UserId { get; set; }
        public User User { get; set; } = default!;
        public List<PromptResponse>? PromptResponses { get; set; } = new();
    }
}