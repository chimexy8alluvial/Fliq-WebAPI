using Fliq.Api.Controllers;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Contracts.Profile;
using Fliq.Application.MatchedProfile.Commands.Create;
using Fliq.Application.MatchedProfile.Commands.MatchedList;
using Fliq.Contracts.MatchedProfile;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("initiateMatch")]
        public async Task<IActionResult> Initiate_Match([FromBody] CreateMatchRequest request)
        {
            var MatchInitiatoruserId = GetAuthUserId();
            var modifiedRequest = request with { MatchInitiatorUserId = MatchInitiatoruserId };
            var command = _mapper.Map<CreateMatchProfileCommand>(modifiedRequest);
           
            var matchedProfileResult = await _mediator.Send(command);

            return matchedProfileResult.Match(
                profileResult => Ok(_mapper.Map<MatchedProfileResponse>(matchedProfileResult)),
                errors => Problem(errors)
            );

        }

        [HttpGet("GetMatchedList")]
        public async Task<IActionResult> GetMatchedList()
        {
            var userId = GetAuthUserId();
            var command = _mapper.Map<CreateMatchListCommand>(userId);
            
            var matchelistResult = await _mediator.Send(command);

            return matchelistResult.Match(
                profileResult => Ok(_mapper.Map<MatchedProfileResponse>(matchelistResult)),
                errors => Problem(errors)
            );
        }
    }
}
