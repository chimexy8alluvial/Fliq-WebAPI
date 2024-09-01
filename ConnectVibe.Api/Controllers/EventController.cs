using ConnectVibe.Api.Controllers;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using MapsterMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using Fliq.Contracts.Event;
using ConnectVibe.Application.Authentication.Commands.ChangePassword;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;
using Fliq.Application.Event.Commands.Create;

namespace Fliq.Api.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IOtpRepository _otpRepository;
        private readonly ILoggerManager _logger;

        public EventController(ISender mediator, IMapper mapper, IOtpRepository otpRepository, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _logger = logger;
        }

        [HttpPost("create eventDetails")]
        public async Task<IActionResult> EventDetails([FromBody] CreateEventDetailsRequest request)
        {
            _logger.LogInfo($"Event Details Request Received: {request}");
            var command = _mapper.Map<CreateEventDetailsCommand>(request);
            var EventResult = await _mediator.Send(command);
            _logger.LogInfo($"EventDetails command Executed. Result: {EventResult}");

            return EventResult.Match(
               EventResult => Ok(_mapper.Map<CreateEventDetailsResponse>(EventResult)),
               errors => Problem(errors)
           );

        }
    }
}
