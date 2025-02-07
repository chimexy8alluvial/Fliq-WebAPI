using Dapper;
using Fliq.Infrastructure.Persistence;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Claims;

namespace Fliq.Api.Common.Middlewares
{
    public class ActivityTrackingMiddleware
    {
        private readonly RequestDelegate _next;  // Stores the next middleware in the pipeline
        private readonly string _connectionString;

        public ActivityTrackingMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _connectionString = configuration.GetConnectionString("FliqDbContext")!;
        }

        public async Task Invoke(HttpContext context, FliqDbContext dbContext)
        {
            // Check if user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userId, out var userIdInt)) // Ensure userId is a valid integer
                {
                    using var connection = new SqlConnection(_connectionString);
                    await connection.ExecuteAsync(
                        "sp_UpdateUserLastActiveAt",
                        new { UserId = userIdInt, LastActiveAt = DateTime.UtcNow },
                        commandType: CommandType.StoredProcedure
                    );
                }
            }

            await _next(context);
        }
    }

    public static class ActivityTrackingMiddlewareExtension
    {
        public static IApplicationBuilder UseActivityTrackingMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ActivityTrackingMiddleware>();
        }
    }
}
