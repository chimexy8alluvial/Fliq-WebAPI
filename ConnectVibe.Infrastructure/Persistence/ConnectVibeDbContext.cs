using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Notifications;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Prompts;
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
        public DbSet<Stake> Stakes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(18, 2);
            modelBuilder.Entity<EventTicket>()
       .HasOne(e => e.User)
       .WithMany()
       .HasForeignKey(e => e.UserId)
       .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<EventTicket>()
                .HasOne(e => e.Payment)
                .WithMany()
                .HasForeignKey(e => e.PaymentId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventTicket>()
                .HasOne(e => e.Ticket)
                .WithMany()
                .HasForeignKey(e => e.TicketId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}