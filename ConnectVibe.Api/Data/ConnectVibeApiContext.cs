using Microsoft.EntityFrameworkCore;

namespace ConnectVibe.Api.Data
{
    public class ConnectVibeApiContext : DbContext
    {
        public ConnectVibeApiContext(DbContextOptions<ConnectVibeApiContext> options)
            : base(options)
        {
        }

        public DbSet<Values> Values { get; set; } = default!;
    }
}
