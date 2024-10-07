using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static Google.Apis.Auth.OAuth2.Web.AuthorizationCodeWebApp;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ExploreController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public ExploreController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Produces(typeof(ExploreResponse))]
        public async Task<IActionResult> Explore([FromQuery] ExploreRequest request)
        {
            _logger.LogInfo($"Exploring profiles request recieved: {request}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            // Map request to ExploreQuery and add UserId
            var query = _mapper.Map<ExploreQuery>(request);
            query = query with { UserId = userId };
            
            var result = await _mediator.Send(query);
            _logger.LogInfo($"Explore Query Executed. Result: {result}");

            return result.Match(
                    result => Ok(_mapper.Map<ExploreResponse>(result)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );

        }

        [HttpGet]
        [Produces(typeof(ExploreResponse))]
        public async Task<IActionResult> Explore([FromQuery] ExploreEventsRequest request)
        {
            _logger.LogInfo($"Exploring Events request received: {request}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            // Map request to ExploreQuery and add UserId
            var query = _mapper.Map<ExploreEventsQuery>(request);
            query = query with { UserId = userId };

            var result = await _mediator.Send(query);
            _logger.LogInfo($"Explore Events Query Executed. Result: {result}");

            return result.Match(
                    result => Ok(_mapper.Map<ExploreEventsResponse>(result)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );

        }

    }
}
