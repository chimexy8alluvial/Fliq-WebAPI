using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Contracts.Event;
using Fliq.Contracts.Event.UpdateDtos;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    //[Authorize]
    //[Route("api/event")]
    //[ApiController]
    [Route("api/event")]
    [AllowAnonymous]
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

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request)
        {
            _logger.LogInfo($"Create Request Received: {request}");
            var command = _mapper.Map<CreateEventCommand>(request);
            var EventCreatedResult = await _mediator.Send(command);
            _logger.LogInfo($"EventCreatedResult command Executed. Result: {EventCreatedResult}");

            return EventCreatedResult.Match(
                CreateEventResult => Ok(_mapper.Map<CreateEventResponse>(EventCreatedResult)),
                errors => Problem(errors)
            );
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateEventDto request)
        {
            _logger.LogInfo($"Update Request  Received: {request}");
            var command = _mapper.Map<UpdateEventCommand>(request);
            var EventCreatedResult = await _mediator.Send(command);
            _logger.LogInfo($"Update command Executed. Result: {EventCreatedResult}");

            return EventCreatedResult.Match(
                CreateEventResult => Ok(_mapper.Map<GetEventResponse>(EventCreatedResult)),
                errors => Problem(errors)
            );
        }
    }
}