using ConnectVibe.Api.Controllers;
using Fliq.Application.Settings.Commands.Update;
using Fliq.Contracts.Settings;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    public class SettingsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public SettingsController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] UpdateSettingsRequest request)
        {
            var command = _mapper.Map<UpdateSettingsCommand>(request);
            var settingsResult = await _mediator.Send(command);
            return settingsResult.Match(
                settingsResult => Ok(_mapper.Map<GetSettingsResponse>(settingsResult)),
                errors => Problem(errors)
            );
        }
    }
}