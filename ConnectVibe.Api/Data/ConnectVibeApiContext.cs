using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ConnectVibe.Api;

namespace ConnectVibe.Api.Data
{
    public class ConnectVibeApiContext : DbContext
    {
        public ConnectVibeApiContext (DbContextOptions<ConnectVibeApiContext> options)
            : base(options)
        {
        }

        public DbSet<ConnectVibe.Api.Values> Values { get; set; } = default!;
    }
}
