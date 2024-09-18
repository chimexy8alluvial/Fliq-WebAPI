using ConnectVibe.Domain.Entities;
using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities;
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
        public DbSet<LocationDetail> LocationDetails { get; set; } = null!;

        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
    }
}