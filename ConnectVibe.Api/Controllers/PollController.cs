using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Application.Poll.Commands.CreatePoll;
using Fliq.Application.Poll.Commands.Voting;
using Fliq.Application.Poll.Queries.GetVotingList;
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
        public async Task<IActionResult> CreatePoll([FromBody] PollRequest request)
        {
            _logger.LogInfo($"Create Poll Request Received: {request}");
            var pollRequestinitiatorId = GetAuthUserId();
            var modifiedRequest = request with { UserId = pollRequestinitiatorId };
            var command = _mapper.Map<CreatePollCommand>(modifiedRequest);

            var createpollResult = await _mediator.Send(command);
            _logger.LogInfo($"Create Poll Request Command Executed. Result: {createpollResult}");

            return createpollResult.Match(
               createpollResult => Ok(_mapper.Map<CreatePollResponse>(createpollResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("vote")]
        public async Task<IActionResult> vote([FromBody] PollRequest request)
        {
            _logger.LogInfo($"Vote Request Received: {request}");
            var voteinitiatorId = GetAuthUserId();
            var modifiedRequest = request with { UserId = voteinitiatorId };
            var command = _mapper.Map<VoteCommand>(modifiedRequest);

            var VoteResult = await _mediator.Send(command);
            _logger.LogInfo($"Vote Request Command Executed. Result: {VoteResult}");

            return VoteResult.Match(
               VoteResult => Ok(_mapper.Map<VoteResponse>(VoteResult)),
                errors => Problem(errors)
            );
        }

        [HttpGet("votinglist")]
        public async Task<IActionResult> VotingList()
        {
            var voteinitiatorId = GetAuthUserId();

            _logger.LogInfo($"Vote Request Received: {voteinitiatorId}");
            var requestList = new VotingListCommand(voteinitiatorId);
            var command = _mapper.Map<VoteCommand>(requestList);

            var VoteListResult = await _mediator.Send(command);
            _logger.LogInfo($" Command Executed. Result: {VoteListResult}");

            return VoteListResult.Match(
               VoteListResult => Ok(_mapper.Map<VoteResponse>(VoteListResult)),
                errors => Problem(errors)
            );
        }
    }
}
