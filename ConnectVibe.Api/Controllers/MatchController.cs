using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.AcceptedMatch;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IOtpRepository _otpRepository;
        private readonly ILoggerManager _logger;

        public MatchController(ISender mediator, IMapper mapper, IOtpRepository otpRepository, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _logger = logger;
        }

        [HttpPost("initiatematch")]
        public async Task<IActionResult> Initiate_Match([FromForm] CreateMatchRequest request)
        {
            _logger.LogInfo($"Initiate Match Request Received: {request}");
            var MatchInitiatoruserId = GetAuthUserId();
            var modifiedRequest = request with { MatchInitiatorUserId = MatchInitiatoruserId };
            var command = _mapper.Map<InitiateMatchRequestCommand>(modifiedRequest);

            var matchedProfileResult = await _mediator.Send(command);
            _logger.LogInfo($"Initiate Match Request Command Executed.  Result: {matchedProfileResult}");

            return matchedProfileResult.Match(
                matchedProfileResult => Ok(_mapper.Map<MatchedProfileResponse>(matchedProfileResult)),
                errors => Problem(errors)
            );
        }

        [HttpGet("getmatchedlist")]
        public async Task<IActionResult> GetMatchedList()
        {
            var userId = GetAuthUserId();
            _logger.LogInfo($"Get Match List Request Received: {userId}");
            var requestList = new GetMatchRequestListCommand(userId);

            var matchelistResult = await _mediator.Send(requestList);
            _logger.LogInfo($"Get Match List Request Command Executed.  Result: {matchelistResult}");
            return matchelistResult.Match(
                matchelistResult => Ok(_mapper.Map<List<MatchedProfileResponse>>(matchelistResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("acceptmatch")]
        //just accept match request
        public async Task<IActionResult> Accept([FromBody] CreateAcceptMatchRequest request)
        {
            _logger.LogInfo($"Accept Match Request Received: {request}");
            var userId = GetAuthUserId();
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<AcceptMatchRequestCommand>(modifiedRequest);

            var acceptMatchResult = await _mediator.Send(command);
            _logger.LogInfo($"Accept Match Request Command Executed.  Result: {acceptMatchResult}");

            return acceptMatchResult.Match(
                acceptMatchResult => Ok(_mapper.Map<MatchedProfileResponse>(acceptMatchResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("rejectmatch")]
        public async Task<IActionResult> Reject([FromBody] CreateAcceptMatchRequest request)
        {
            _logger.LogInfo($"Accept Match Request Received: {request}");
            var userId = GetAuthUserId();
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<AcceptMatchRequestCommand>(modifiedRequest);

            var acceptMatchResult = await _mediator.Send(command);
            _logger.LogInfo($"Accept Match Request Command Executed.  Result: {acceptMatchResult}");

            return acceptMatchResult.Match(
                acceptMatchResult => Ok(_mapper.Map<MatchedProfileResponse>(acceptMatchResult)),
                errors => Problem(errors)
            );
        }
    }
}
