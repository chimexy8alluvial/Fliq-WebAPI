using Fliq.Application.AuditTrail.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Fliq.Infrastructure.Services
{
    public class AuditTrailService : IAuditTrailService
    {
        private readonly IAuditTrailRepository _auditTrailRepository;
        private readonly IHttpContextAccessor _contextAccessor;
        public AuditTrailService(IAuditTrailRepository auditTrailRepository, IHttpContextAccessor contextAccessor)
        {
            _auditTrailRepository = auditTrailRepository;
            _contextAccessor = contextAccessor;
        }

        public async Task LogAuditTrail(string? Message, User User)
        {
            var ipAddress = _contextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";

            var auditTrail = new AuditTrail
            {
                UserId = User.Id,
                UserFirstName = User.FirstName,
                UserLastName = User.LastName,
                UserEmail = User.Email,
                UserRole = User.Role.Name,
                AuditAction = Message,
                IPAddress = ipAddress
            };

            await _auditTrailRepository.AddAuditTrailAsync(auditTrail);
        }
    }
}
