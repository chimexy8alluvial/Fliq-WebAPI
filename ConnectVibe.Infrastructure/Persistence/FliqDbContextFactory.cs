using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Fliq.Infrastructure.Persistence
{
    public class FliqDbContextFactory : IDesignTimeDbContextFactory<FliqDbContext>
    {
        public FliqDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<FliqDbContext>();
            optionsBuilder.UseSqlServer("Server=LAPTOP-0VSJ0RU3;Database=FliqTest;TrustServerCertificate=True;Trusted_Connection=True;MultipleActiveResultSets=true");

            return new FliqDbContext(optionsBuilder.Options);
        }
    }
}
