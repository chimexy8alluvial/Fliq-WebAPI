using Fliq.Application.Authentication.Queries.Login;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.PlatformCompliance.Commands;
using Fliq.Application.PlatformCompliance.Queries;
using Fliq.Contracts.PlatformCompliance;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComplianceController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public ComplianceController(ISender mediator, ILoggerManager logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public async Task<IActionResult> CreateCompliance(CreateComplianceRequest request)
        {
            _logger.LogInfo("Received request to create compliance.");
            
            var command = _mapper.Map<CreateComplianceCommand>(request);

            var result = await _mediator.Send(command);
            _logger.LogInfo("Record User consent executed successfully.");
            return result.Match(
                success => Ok(success),
                errors => Problem(errors));
        }

        [HttpPost("consent")]
        [Authorize]
        public async Task<IActionResult> RecordConsent(RecordUserConsentRequest request)
        {
            // Get IP address from request
            string ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            _logger.LogInfo($"Recording consent from IP: {ipAddress}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var command = _mapper.Map<RecordUserConsentCommand>(request);

            // Override the IP and userId in the command
            var cmdWithIp = command with { IPAddress = ipAddress, UserId = userId };

            var result = await _mediator.Send(cmdWithIp);

            _logger.LogInfo("Record User consent executed successfully.");

            return result.Match(
                _ => Ok(),
                errors => Problem(errors));
        }

        [HttpGet("consent/status")]
        [Authorize]
        public async Task<IActionResult> GetConsentStatus([FromQuery] int userId, [FromQuery] int ComplianceTypeId)
        {
            _logger.LogInfo($"Fetching consent status for user {userId}, compliance type {ComplianceTypeId}");
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
            _logger.LogInfo($"Fetching consent history for user {userId}, compliance type: {(ComplianceTypeId.HasValue ? ComplianceTypeId.ToString() : "All")}");
            var query = new GetUserConsentHistoryQuery(userId, ComplianceTypeId);
            var result = await _mediator.Send(query);

            return result.Match(
                history => Ok(history),
                errors => Problem(errors));
        }
    }
}