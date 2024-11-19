using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Poll.Commands.CreatePoll;
using Fliq.Contracts.Polls;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PollController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IOtpRepository _otpRepository;
        private readonly ILoggerManager _logger;

        public PollController(ISender mediator, IMapper mapper, IOtpRepository otpRepository, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _logger = logger;
        }

        [HttpPost("createpoll")]
        public async Task<IActionResult> CreatePoll([FromBody] VoteRequest request)
        {
            _logger.LogInfo($"Create Poll Request Received: {request}");
            var pollRequestinitiatorId = GetAuthUserId();
            var modifiedRequest = request with { UserId = pollRequestinitiatorId };
            var command = _mapper.Map<CreatePollCommand>(modifiedRequest);

            var createpollResult = await _mediator.Send(command);
            _logger.LogInfo($"Create Poll Request Command Executed. Result: {createpollResult}");

            //return createpollResult.Match(
            //   createpollResult => Ok(_mapper.Map<PollRequestResponse>(createpollResult)),
            //    errors => Problem(errors)
            //);
            return Ok(createpollResult);
        }
    }
}
