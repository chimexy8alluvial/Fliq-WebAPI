using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.AcceptedMatch;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Commands.RejectMatch;
using Fliq.Contracts.MatchedProfile;
using Fliq.Application.Common.Pagination;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Fliq.Application.MatchedProfile.Common;

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
        public async Task<IActionResult> Initiate_Match([FromForm] MatchRequest request)
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
        public async Task<IActionResult> GetMatchedList([FromQuery] MatchListRequest request)
        {
            var userId = GetAuthUserId();
            _logger.LogInfo($"Get Match List Request Received: {userId}");

            //Map the request to GetMatchRequestListCommand and add UserId
            var query = _mapper.Map<GetMatchRequestListCommand>(request);
            query = query with { UserId = userId, PaginationRequest = new PaginationRequest(request.PageNumber, request.PageSize) };

            var matchelistResult = await _mediator.Send(query);
            _logger.LogInfo($"Get Match List Request Command Executed.  Result: {matchelistResult}");
            return matchelistResult.Match(
                matchelistResult => Ok(_mapper.Map<List<MatchedProfileResponse>>(matchelistResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("acceptmatch")]
        public async Task<IActionResult> Accept([FromBody] AcceptMatchRequest request)
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
        public async Task<IActionResult> Reject([FromBody] RejectMatchRequest request)
        {
            _logger.LogInfo($"Accept Match Request Received: {request}");
            var userId = GetAuthUserId();
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<RejectMatchRequestCommand>(modifiedRequest);

            var rejectMatchResult = await _mediator.Send(command);
            _logger.LogInfo($"Accept Match Request Command Executed.  Result: {rejectMatchResult}");

            return rejectMatchResult.Match(
                rejectMatchResult => Ok(_mapper.Map<MatchedProfileResponse>(rejectMatchResult)),
                errors => Problem(errors)
            );
        }
    }
}