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
        public List<ProfilePhoto> Photos { get; set; } = new();

        [Column("Passions")]         //JSON string of Passions in the database
        public string PassionsJson { get; set; } = "[]"; 
        
        [NotMapped]   
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
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}