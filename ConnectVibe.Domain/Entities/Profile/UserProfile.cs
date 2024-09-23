using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Enums;

namespace ConnectVibe.Domain.Entities.Profile
{
    public class UserProfile
    {
        public int Id { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
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
        public List<ProfileType> ProfileTypes { get; set; } = new();
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}