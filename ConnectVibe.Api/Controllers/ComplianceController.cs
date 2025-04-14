using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.PlatformCompliance.Commands;
using Fliq.Application.PlatformCompliance.Queries;
using Fliq.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    public class ComplianceController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly ILoggerManager _logger;

        public ComplianceController(ISender mediator, ILoggerManager logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateCompliance(CreateComplianceCommand command)
        {
            var result = await _mediator.Send(command);

            return result.Match(
                success => Ok(success),
                errors => Problem(errors));
        }

        [HttpPost("consent")]
        [Authorize]
        public async Task<IActionResult> RecordConsent(RecordUserConsentCommand command)
        {
            // Get IP address from request
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            // Override the IP in the command
            var cmdWithIp = command with { IPAddress = ipAddress };

            var result = await _mediator.Send(cmdWithIp);

            return result.Match(
                _ => Ok(),
                errors => Problem(errors));
        }

        [HttpGet("consent/status")]
        [Authorize]
        public async Task<IActionResult> GetConsentStatus([FromQuery] int userId, [FromQuery] int ComplianceTypeId)
        {
            var query = new GetUserConsentStatusQuery(userId, ComplianceTypeId);
            var result = await _mediator.Send(query);

            return result.Match(
                status => Ok(status),
                errors => Problem(errors));
        }

        [HttpGet("consent/history")]
        [Authorize]
        public async Task<IActionResult> GetConsentHistory([FromQuery] int userId, [FromQuery] int? ComplianceTypeId = null)
        {
            var query = new GetUserConsentHistoryQuery(userId, ComplianceTypeId);
            var result = await _mediator.Send(query);

            return result.Match(
                history => Ok(history),
                errors => Problem(errors));
        }
    }
}