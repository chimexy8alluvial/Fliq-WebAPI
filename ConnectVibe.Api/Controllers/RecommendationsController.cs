using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Commands;
using Fliq.Application.Recommendations.Queries;
using Fliq.Contracts.Event.UpdateDtos;
using Fliq.Contracts.Recommendations;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public RecommendationsController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("events")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedEvents([FromQuery] int count = 10)
        {
            _logger.LogInfo($"Get Recommended Events Query received to fetch {count} recommended events");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");
            var query = new GetRecommendedEventsQuery(userId, count);

            var result = await _mediator.Send(query);

            return result.Match(
                result => Ok(_mapper.Map<List<GetEventResponse>>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("blinddates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedBlindDates([FromQuery] int count = 10)
        {
            _logger.LogInfo($"Get Recommended BlindDates Query received to fetch {count} recommended blind dates");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");
            var query = new GetRecommendedBlindDatesQuery(userId, count);

            var result = await _mediator.Send(query);

            return result.Match(
                blindDates => Ok(/*_mapper.Map<List<GetBlindDateResponse>>*/(blindDates)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("speeddates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedSpeedDates([FromQuery] int count = 10)
        {
            _logger.LogInfo($"Get Recommended SpeedDates Query received to fetch {count} recommended speed dates");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");
            var query = new GetRecommendedSpeedDatesQuery(userId, count);

            var result = await _mediator.Send(query);

            return result.Match(
                speedDates => Ok(/*_mapper.Map<List<SpeedDatingEventResponse>>*/(speedDates)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedUsers([FromQuery] int count = 10)
        {
            _logger.LogInfo($"Get Recommended Users Query received to fetch {count} recommended users");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");
            var query = new GetRecommendedUsersQuery(userId, count);

            var result = await _mediator.Send(query);

            return result.Match(
                users => Ok((users)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("track")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TrackInteraction([FromBody] TrackUserInteractionRequest request)
        {
            _logger.LogInfo("TrackInteraction endpoint called.");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var command = new TrackUserInteractionCommand(
                userId,
                request.InteractionType,
                request.EventId,
                request.BlindDateId,
                request.SpeedDatingEventId,
                request.InteractedWithUserId,
                request.InteractionStrength
            );

            var result = await _mediator.Send(command);

            return result.Match(
                result => Ok($"User Interaction with {request.InteractionType} tracked successfully"),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }
    }
}
