using ConnectVibe.Application.Common.Interfaces.Services;

namespace ConnectVibe.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
