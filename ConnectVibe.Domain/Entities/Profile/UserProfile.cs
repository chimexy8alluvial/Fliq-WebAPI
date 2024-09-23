namespace Fliq.Domain.Entities.Profile
{
    public class UserProfile
    {
        public int Id { get; set; }
        public DateTime DOB { get; set; }
        public Gender Gender { get; set; } = default!;
        public SexualOrientation SexualOrientation { get; set; } = default!;
        public Religion Religion { get; set; } = default!;
        public Ethnicity Ethnicity { get; set; } = default!;
        public HaveKids HaveKids { get; set; } = default!;
        public WantKids WantKids { get; set; } = default!;
        public Location Location { get; set; } = default!;
        public bool AllowNotifications { get; set; }
        public List<string> Passions { get; set; } = default!;
        public List<ProfilePhoto> Photos { get; set; } = default!;
        public int UserId { get; set; }
        public User User { get; set; } = default!;
    }
}