

using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Services
{
    public interface IAuditTrailService
    {
        Task LogAuditTrail(string? Message, User User);
    }
}
