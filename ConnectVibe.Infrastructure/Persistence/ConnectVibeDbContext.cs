using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.VotingPoll;

namespace Fliq.Infrastructure.Persistence
{
    public class FliqDbContext : DbContext
    {
        public FliqDbContext(DbContextOptions<FliqDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OTP> OTPs { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<MatchRequest> MatchRequests { get; set; }
        public DbSet<VotePoll> VotePolls { get; set; } = null!;

        public DbSet<LocationDetail> LocationDetails { get; set; }
       

        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2); // Example: 18 digits total, 2 after decimal
        }
    }
}