using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities;

namespace ConnectVibe.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public bool IsEmailValidated { get; set; }

        public UserProfile? UserProfile { get; set; }
        public List<Payment>? Payments { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
    }
}