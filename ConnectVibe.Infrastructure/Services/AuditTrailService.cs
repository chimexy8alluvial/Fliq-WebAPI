using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Fliq.Application.AuditTrailCommand;

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
            //var ipAddress = context. // <-- Use the extension method
            var auditTrail = new AuditTrail
            {
                UserId = User.Id,
                UserFirstName = User.FirstName,
                UserLastName = User.LastName,
                UserEmail = User.Email,
                UserRole = User.Role.Name,
                AuditAction = Message,
                IPAddress = ""
            };

            await _auditTrailRepository.AddAuditTrailAsync(auditTrail);
        }
    }
}
