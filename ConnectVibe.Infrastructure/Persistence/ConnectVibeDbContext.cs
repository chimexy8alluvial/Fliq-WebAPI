using ConnectVibe.Domain.Entities;
using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event;
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
        public DbSet<Events> Events { get; set; }
        public DbSet<EventMedia> EventMedias { get; set; }
        public DbSet<EventCriteria> EventCriterias { get; set; }
        public DbSet<SponsoredEventDetail> SponsoredEventDetails { get; set; }
        public DbSet<TicketType> TicketTypes { get; set; }

       
    } 
};

