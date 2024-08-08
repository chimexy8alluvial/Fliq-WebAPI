using ConnectVibe.Domain.Entities;
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
    }
}
