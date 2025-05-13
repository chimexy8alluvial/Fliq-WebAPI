using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.UserFeatureActivities;
using Fliq.Application.MatchedProfile.Commands.AcceptedMatch;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Commands.RejectMatch;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.MatchedProfile.Queries;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MatchController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IUserFeatureActivityService _userFeatureActivityService;
        public MatchController(ISender mediator, IMapper mapper, ILoggerManager logger, IUserFeatureActivityService userFeatureActivityService)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
            _userFeatureActivityService = userFeatureActivityService;
        }

        [HttpPost("initiate-match")]
        public async Task<IActionResult> Initiate_Match([FromForm] MatchRequest request)
        {
            _logger.LogInfo($"Initiate Match Request Received: {request}");
            var matchInitiatorUserId = GetAuthUserId();

            await _userFeatureActivityService.TrackUserFeatureActivity(matchInitiatorUserId, "Initiate-MatchRequest");

            var command = _mapper.Map<InitiateMatchRequestCommand>(request);
            command.MatchInitiatorUserId = matchInitiatorUserId;

            var matchedProfileResult = await _mediator.Send(command);
            if (matchedProfileResult.IsError)
            {
                var errorMessages = string.Join("; ", matchedProfileResult.Errors.Select(e => e.Description));
                _logger.LogError($"Match request failed: {errorMessages}");
            }
            else
            {
                var result = matchedProfileResult.Value;
                var resultString = $"Id={result.Id}, ReceiverId={result.RequestedUserId}, InitiatorId={result.MatchInitiatorUserId}, Status={result.MatchRequestStatus}";
                _logger.LogInfo($"Match request succeeded: {resultString}");
            }

            return matchedProfileResult.Match(
                matchedProfileResult => Ok(_mapper.Map<MatchRequestResponse>(matchedProfileResult)),
                errors => Problem(errors)
            );
        }

        [HttpGet("match-requests")]
        public async Task<IActionResult> GetMatchedList(GetMatchListRequest request)
        {
            var userId = GetAuthUserId();
            _logger.LogInfo($"Get Match List Request Received: {userId}");

            var query = _mapper.Map<GetMatchRequestListCommand>(request);
            query = query with { UserId = userId};

            var matchRequestResult = await _mediator.Send(query);
            _logger.LogInfo($"Get Match List Request Command Executed.  Result: {matchRequestResult}");
            return matchRequestResult.Match(
                matchRequestResult => Ok(_mapper.Map<List<MatchRequestResponse>>(matchRequestResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("accept")]
        public async Task<IActionResult> Accept([FromBody] AcceptMatchRequest request)
        {
            _logger.LogInfo($"Accept Match Request Received: {request}");
            var userId = GetAuthUserId();

            await _userFeatureActivityService.TrackUserFeatureActivity(userId, "Accept-MatchRequest");

            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<AcceptMatchRequestCommand>(modifiedRequest);

            var acceptMatchResult = await _mediator.Send(command);
            var resultString = acceptMatchResult.IsError
                ? $"Errors: {string.Join("; ", acceptMatchResult.Errors.Select(e => e.Description))}"
                : $" ReceiverId={userId}, InitiatorId={acceptMatchResult.Value.MatchInitiatorUserId}";
            _logger.LogInfo($"Accept Match Request Command Executed. Result: {resultString}");

            return acceptMatchResult.Match(
                acceptMatchResult => Ok(new
                {
                    Message = "Match accepted successfully",
                    Data = _mapper.Map<MatchRequestResponse>(acceptMatchResult)
                }),
                errors => Problem(errors)
            );
        }
        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] RejectMatchRequest request)
        {
            _logger.LogInfo($"Reject Match Request Received: {request}");
            var userId = GetAuthUserId();

            await _userFeatureActivityService.TrackUserFeatureActivity(userId, "Reject-MatchRequest");

            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<RejectMatchRequestCommand>(modifiedRequest);

            var rejectMatchResult = await _mediator.Send(command);
            var resultString = rejectMatchResult.IsError
                ? $"Errors: {string.Join("; ", rejectMatchResult.Errors.Select(e => e.Description))}"
                : $" ReceiverId={userId}, InitiatorId={rejectMatchResult.Value.MatchInitiatorUserId}";
            _logger.LogInfo($"Reject Match Request Command Executed. Result: {resultString}");

            return rejectMatchResult.Match(
                rejectMatchResult => Ok(new
                {
                    Message = "Match rejected successfully",
                    Data = _mapper.Map<MatchRequestResponse>(rejectMatchResult)
                }),
                errors => Problem(errors)
            );
        }

        //----------- Admin Fxns -----------
        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("recent-user-matches")]
        public async Task<IActionResult> GetRecentUserMatches([FromQuery] GetRecentUsersMatchRequest request)
        {
            _logger.LogInfo($"Get Recent Users Match Request Received: UserId={request.UserId}, Limit={request.Limit}, Status={(request.Status.HasValue ? request.Status.ToString() : "Any")}");
            var adminId = GetAuthUserId();

            var query = _mapper.Map<GetRecentUsersMatchQuery>(request) with { AdminUserId = adminId };
            _logger.LogInfo($"Mapped Query: AdminUserId={query.AdminUserId}, UserId={query.UserId}, Limit={query.Limit}, Status={(query.Status.HasValue ? query.Status.ToString() : "Any")}");

            var result = await _mediator.Send(query);
            var resultString = result.IsError
                ? $"Errors: {string.Join("; ", result.Errors.Select(e => e.Description))}"
                : $"Count={result.Value?.Count ?? 0}";
            _logger.LogInfo($"Get Recent Users Match Query Executed. Result: {resultString}");

            return result.Match(
                result => Ok(_mapper.Map<GetRecentUsersMatchResponse>(result)),
                errors => Problem(errors)
            );
        }
    }
}