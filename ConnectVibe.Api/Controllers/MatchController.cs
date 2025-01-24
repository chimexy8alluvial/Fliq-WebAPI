﻿using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.AcceptedMatch;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.MatchedProfile.Commands.RejectMatch;
using Fliq.Application.MatchedProfile.Common;
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
        private readonly ILoggerManager _logger;

        public MatchController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("initiate-match")]
        public async Task<IActionResult> Initiate_Match([FromForm] MatchRequest request)
        {
            _logger.LogInfo($"Initiate Match Request Received: {request}");
            var matchInitiatorUserId = GetAuthUserId();
            var modifiedRequest = request with { MatchInitiatorUserId = matchInitiatorUserId };
            var command = _mapper.Map<InitiateMatchRequestCommand>(modifiedRequest);

            var matchedProfileResult = await _mediator.Send(command);
            _logger.LogInfo($"Initiate Match Request Command Executed.  Result: {matchedProfileResult}");

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
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<AcceptMatchRequestCommand>(modifiedRequest);

            var acceptMatchResult = await _mediator.Send(command);
            _logger.LogInfo($"Accept Match Request Command Executed.  Result: {acceptMatchResult}");

            return acceptMatchResult.Match(
                acceptMatchResult => Ok(_mapper.Map<MatchRequestResponse>(acceptMatchResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("reject")]
        public async Task<IActionResult> Reject([FromBody] RejectMatchRequest request)
        {
            _logger.LogInfo($"Accept Match Request Received: {request}");
            var userId = GetAuthUserId();
            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<RejectMatchRequestCommand>(modifiedRequest);

            var rejectMatchResult = await _mediator.Send(command);
            _logger.LogInfo($"Accept Match Request Command Executed.  Result: {rejectMatchResult}");

            return rejectMatchResult.Match(
                rejectMatchResult => Ok(_mapper.Map<MatchRequestResponse>(rejectMatchResult)),
                errors => Problem(errors)
            );
        }
    }
}