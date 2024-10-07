using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Commands.Update;
using Fliq.Application.Settings.Queries.GetSettings;
using Fliq.Contracts.Settings;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/settings")]
    public class SettingsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public SettingsController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInfo($"Get Settings Request Received");
            var userId = GetAuthUserId();
            var query = new GetSettingsQuery(userId);
            var settingsResult = await _mediator.Send(query);
            _logger.LogInfo($"Fetch Settings Query Executed. Result: {settingsResult}");
            return settingsResult.Match(
                settingsResult => Ok(_mapper.Map<GetSettingsResponse>(settingsResult)),
                errors => Problem(errors)
            );
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSettingsRequest request)
        {
            _logger.LogInfo($"Update Settings Request Received: {request}");
            var userId = GetAuthUserId();
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<UpdateSettingsCommand>(modifiedRequest);
            var settingsResult = await _mediator.Send(command);
            _logger.LogInfo($"Update Settings Command Executed. Result: {settingsResult}");
            return settingsResult.Match(
                settingsResult => Ok(_mapper.Map<GetSettingsResponse>(settingsResult)),
                errors => Problem(errors)
            );
        }
    }
}