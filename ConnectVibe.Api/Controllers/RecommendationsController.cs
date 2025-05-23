using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Recommendations.Commands;
using Fliq.Application.Recommendations.Queries;
using Fliq.Contracts.Event.UpdateDtos;
using Fliq.Contracts.Recommendations;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Enums.Recommendations;
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
        public async Task<IActionResult> GetRecommendedEvents([FromQuery] int count = 10, [FromQuery] bool forceRefresh = false)
        {
            _logger.LogInfo($"Get Recommended Events Query received to fetch {count} recommended events");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            // Try cache first unless force refresh is requested
            if (!forceRefresh)
            {
                var cachedResult = await _mediator.Send(new GetCachedRecommendationsQuery(userId, RecommendationType.Event, count));

                if (cachedResult.IsError == false && cachedResult.Value is List<Events> cachedEvents && cachedEvents.Any())
                {
                    _logger.LogInfo($"Returning {cachedEvents.Count} cached event recommendations for user {userId}");
                    return Ok(_mapper.Map<List<GetEventResponse>>(cachedEvents));
                }
            }

            // Fallback to real-time calculation
            _logger.LogInfo($"No cached recommendations found, calculating real-time for user {userId}");
            var result = await _mediator.Send(new GetRecommendedEventsQuery(userId, count));

            return result.Match(
                result => Ok(_mapper.Map<List<GetEventResponse>>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("blinddates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedBlindDates([FromQuery] int count = 10, [FromQuery] bool forceRefresh = false)
        {
            _logger.LogInfo($"Get Recommended BlindDates Query received to fetch {count} recommended blind dates");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            if (!forceRefresh)
            {
                var cachedResult = await _mediator.Send(new GetCachedRecommendationsQuery(userId, RecommendationType.BlindDate, count));

                if (cachedResult.IsError == false && cachedResult.Value is List<BlindDate> cachedBlindDates && cachedBlindDates.Any())
                {
                    _logger.LogInfo($"Returning {cachedBlindDates.Count} cached blind date recommendations for user {userId}");
                    return Ok(cachedBlindDates);
                }
            }

            _logger.LogInfo($"No cached recommendations found, calculating real-time for user {userId}");
            var result = await _mediator.Send(new GetRecommendedBlindDatesQuery(userId, count));

            return result.Match(
                blindDates => Ok(/*_mapper.Map<List<GetBlindDateResponse>>*/(blindDates)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("speeddates")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedSpeedDates([FromQuery] int count = 10, [FromQuery] bool forceRefresh = false)
        {
            _logger.LogInfo($"Get Recommended SpeedDates Query received to fetch {count} recommended speed dates");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            if (!forceRefresh)
            {
                var cachedResult = await _mediator.Send(new GetCachedRecommendationsQuery(userId, RecommendationType.SpeedDate, count));

                if (cachedResult.IsError == false && cachedResult.Value is List<SpeedDatingEvent> cachedSpeedDates && cachedSpeedDates.Any())
                {
                    _logger.LogInfo($"Returning {cachedSpeedDates.Count} cached speed date recommendations for user {userId}");
                    return Ok(cachedSpeedDates);
                }
            }

            _logger.LogInfo($"No cached recommendations found, calculating real-time for user {userId}");
            var result = await _mediator.Send(new GetRecommendedSpeedDatesQuery(userId, count));

            return result.Match(
                speedDates => Ok(/*_mapper.Map<List<SpeedDatingEventResponse>>*/(speedDates)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpGet("users")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetRecommendedUsers([FromQuery] int count = 10, [FromQuery] bool forceRefresh = false)
        {
            _logger.LogInfo($"Get Recommended Users Query received to fetch {count} recommended users");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            if (!forceRefresh)
            {
                var cachedResult = await _mediator.Send(new GetCachedRecommendationsQuery(userId, RecommendationType.User, count));

                if (cachedResult.IsError == false && cachedResult.Value is List<User> cachedUsers && cachedUsers.Any())
                {
                    _logger.LogInfo($"Returning {cachedUsers.Count} cached user recommendations for user {userId}");
                    return Ok(cachedUsers);
                }
            }

            _logger.LogInfo($"No cached recommendations found, calculating real-time for user {userId}");
            var result = await _mediator.Send(new GetRecommendedUsersQuery(userId, count));

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

            // Map interaction type to strength
            double interactionStrength = MapInteractionTypeToStrength(request.InteractionType.ToString());

            var command = new TrackUserInteractionCommand(
                userId,
                request.InteractionType,
                request.EventId,
                request.BlindDateId,
                request.SpeedDatingEventId,
                request.InteractedWithUserId,
                interactionStrength
            );

            var result = await _mediator.Send(command);

            return result.Match(
                result => Ok($"User Interaction with {request.InteractionType} tracked successfully"),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        private double MapInteractionTypeToStrength(string interactionType)
        {
            return interactionType.ToLower() switch
            {
                "view" => 0.1,
                "like" => 0.3,
                "save" => 0.5,
                "attend" => 0.8,
                "review" => 0.7,
                "matchRequest" => 0.8, 
                "match" => 0.9,
                "message" => 0.7,
                _ => 0.2
            };
        }
    }
}
