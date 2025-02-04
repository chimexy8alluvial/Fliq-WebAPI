using Fliq.Infrastructure.Persistence;
using System.Security.Claims;

namespace Fliq.Api.Common.Middlewares
{
    public class ActivityTrackingMiddleware
    {
        private readonly RequestDelegate _next;  // Stores the next middleware in the pipeline

        public ActivityTrackingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, FliqDbContext dbContext)
        {
            // Check if user is authenticated
            if (context.User.Identity?.IsAuthenticated == true)
            {

                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (int.TryParse(userId, out var userIdInt)) // Ensure userId is a valid integer
                {
                    var user = await dbContext.Users.FindAsync(userIdInt); 
                    if (user is not null)
                    {
                        user.LastActiveAt = DateTime.UtcNow;  // Update activity timestamp
                        await dbContext.SaveChangesAsync();
                    }
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
