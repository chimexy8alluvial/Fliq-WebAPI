using Fliq.Application.AuditTrail.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Fliq.Application.AuditTrailCommand;
using System.Net;

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

        public async Task LogAuditTrail(string Message, User User)
        {
            var httpContext = _contextAccessor.HttpContext;
            string ipAddress = string.Empty;

            if (httpContext != null)
            {
                IPAddress remoteIp = GetRemoteIPAddress(httpContext);
                ipAddress = remoteIp?.ToString();

            }
            var auditTrail = new AuditTrail
            {
                UserId = User.Id,
                UserFirstName = User.FirstName,
                UserLastName = User.LastName,
                UserEmail = User.Email,
                UserRole = User.Role?.Name ?? "",
                AuditAction = Message,
                IPAddress = ipAddress ?? ""
            };

            await _auditTrailRepository.AddAuditTrailAsync(auditTrail);
        }

        private IPAddress GetRemoteIPAddress(HttpContext context, bool allowForwarded = true)
        {
            if (allowForwarded)
            {
                string header = (context.Request.Headers["CF-Connecting-IP"].FirstOrDefault() ?? context.Request.Headers["X-Forwarded-For"].FirstOrDefault());
                if (IPAddress.TryParse(header, out IPAddress ip))
                {
                    return ip;
                }
            }
            return context.Connection.RemoteIpAddress;
        }

    }
}
