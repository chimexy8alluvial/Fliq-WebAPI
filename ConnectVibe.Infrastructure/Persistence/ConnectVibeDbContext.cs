using ConnectVibe.Domain.Entities;
using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.MatchedProfile;
using Microsoft.EntityFrameworkCore;

namespace ConnectVibe.Infrastructure.Persistence
{
    public class ConnectVibeDbContext : DbContext
    {
        public ConnectVibeDbContext(DbContextOptions<ConnectVibeDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OTP> OTPs { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<LocationDetail> LocationDetails { get; set; }
        public DbSet<MatchProfile> MatchProfiles { get; set; }
    }
}