using Fliq.Domain.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fliq.Domain.Entities.Profile
{
    public class UserProfile : Record
    {
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
        public string? ProfileDescription { get; set; }  //Nullable
        public SexualOrientation? SexualOrientation { get; set; }  //Nullable
        public Religion Religion { get; set; } = default!;
        public Ethnicity Ethnicity { get; set; } = default!;
        public Occupation Occupation { get; set; } = default!;
        public EducationStatus EducationStatus { get; set; } = default!;
        public HaveKids? HaveKids { get; set; }  //Nullable
        public WantKids? WantKids { get; set; }  //Nullable
        public Location Location { get; set; } = default!;
        public bool AllowNotifications { get; set; }
        public List<string> Passions { get; set; } = new();
        public List<ProfilePhoto> Photos { get; set; } = new();

        // This property will be mapped to the database as a JSON string
        [Column("ProfileTypes")]
        public string ProfileTypeJson { get; set; } = "[]"; // Initialize with an empty JSON array

        // Non-mapped property for convenience to work with enum list
        [NotMapped] // Prevent this from being mapped to the database
        public List<ProfileType> ProfileTypes
        {
            get => JsonConvert.DeserializeObject<List<ProfileType>>(ProfileTypeJson) ?? new List<ProfileType>();
            set => ProfileTypeJson = JsonConvert.SerializeObject(value);
        }
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}