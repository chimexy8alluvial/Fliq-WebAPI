
namespace Fliq.Domain.Entities.UserFeatureActivities
{
    public class UserFeatureActivity : Record
    {
        public int UserId { get; set; }   // Link to User
        public string Feature { get; set; } = string.Empty; // Name of feature
        public DateTime LastActiveAt { get; set; } = DateTime.UtcNow; // Last used time

        public User User { get; set; } = null!; // Navigation property
    }
}
