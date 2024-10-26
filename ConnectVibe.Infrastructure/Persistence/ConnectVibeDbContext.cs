using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;
using Microsoft.EntityFrameworkCore;

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

        public DbSet<Events> Events { get; set; }
        public DbSet<EventMedia> EventMedias { get; set; }
        public DbSet<EventCriteria> EventCriterias { get; set; }
        public DbSet<SponsoredEventDetail> SponsoredEventDetails { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

        public DbSet<LocationDetail> LocationDetails { get; set; } = null!;

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