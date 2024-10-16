using Fliq.Application.Common.Interfaces.Services;

namespace Fliq.Infrastructure.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
