using Fliq.Domain.Entities;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.HelpAndSupport;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Notifications;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Entities.UserFeatureActivities;
using Microsoft.EntityFrameworkCore;

namespace Fliq.Infrastructure.Persistence
{
    public class FliqDbContext : DbContext
    {
        public FliqDbContext(DbContextOptions<FliqDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var fixedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc); // Fixed date

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = 1, Name = "SuperAdmin", DateCreated = fixedDate },
                new Role { Id = 2, Name = "Admin", DateCreated = fixedDate },
                new Role { Id = 3, Name = "User", DateCreated = fixedDate }
            );
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<OTP> OTPs { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;

        public DbSet<Events> Events { get; set; }
        public DbSet<EventMedia> EventMedias { get; set; }
        public DbSet<EventCriteria> EventCriterias { get; set; }
        public DbSet<SponsoredEventDetail> SponsoredEventDetails { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<EventTicket> EventTickets { get; set; }
        public DbSet<EventReview> EventReviews { get; set; }
        public DbSet<Currency> Currencies { get; set; }

        public DbSet<LocationDetail> LocationDetails { get; set; } = null!;
        public DbSet<MatchRequest> MatchRequests { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<UserDeviceToken> UserDeviceTokens { get; set; }

        public DbSet<Subscription> Subscriptions { get; set; } = null!;
        public DbSet<Payment> Payments { get; set; } = null!;
        public DbSet<Setting> Settings { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<GameQuestion> GameQuestions { get; set; } = null!;
        public DbSet<GameSession> GameSessions { get; set; } = null!;
        public DbSet<GameRequest> GameRequests { get; set; }

        public DbSet<PromptCategory> PromptCategories { get; set; }
        public DbSet<PromptQuestion> PromptQuestions { get; set; }

        public DbSet<Wallet> Wallets { get; set; } = null!;
        public DbSet<WalletHistory> WalletHistories { get; set; } = null!;
        public DbSet<Stake> Stakes { get; set; } = null!;

        public DbSet<UserFeatureActivity> UserFeatureActivities { get; set; } = null!;
        public DbSet<SupportTicket> SupportTickets { get; set; } = null!;

        public DbSet<BlindDateCategory> BlindDateCategories { get; set; } = null!;
        public DbSet<BlindDate> BlindDates { get; set; } = null!;
        public DbSet<BlindDateParticipant> BlindDatesParticipants { get; set; } = null!;

        public DbSet<SpeedDatingEvent> SpeedDatingEvents { get; set; } = null!;
        public DbSet<SpeedDatingParticipant> SpeedDatingParticipanticipants { get; set; } = null!;
        public DbSet<Gender> Genders { get; set; }
        public DbSet<WantKids> WantKids { get; set; }
        public DbSet<HaveKids> HaveKids { get; set; }

        public DbSet<AuditTrail> AuditTrails { get; set; }
        public DbSet<BusinessIdentificationDocumentType> BusinessIdentificationDocumentTypes { get; set; }
        public DbSet<BusinessIdentificationDocument> BusinessIdentificationDocuments { get; set; }

    }
}