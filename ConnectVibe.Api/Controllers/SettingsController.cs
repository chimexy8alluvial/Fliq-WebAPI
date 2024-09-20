using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Commands.Update;
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

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSettingsRequest request)
        {
            _logger.LogInfo($"Update Settings Request Received: {request}");
            var command = _mapper.Map<UpdateSettingsCommand>(request);
            var settingsResult = await _mediator.Send(command);
            _logger.LogInfo($"Update Settings Command Executed. Result: {settingsResult}");
            return settingsResult.Match(
                settingsResult => Ok(_mapper.Map<GetSettingsResponse>(settingsResult)),
                errors => Problem(errors)
            );
        }
    }
}