using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Recommendations.Commands
{
    public record TrackUserInteractionCommand(
     int UserId,
     InteractionType InteractionType,
     int? EventId,
     int? BlindDateId,
     int? SpeedDatingEventId,
     int? InteractedWithUserId,
     double InteractionStrength
        ) : IRequest<ErrorOr<Unit>>;

    public class TrackUserInteractionCommandHandler : IRequestHandler<TrackUserInteractionCommand, ErrorOr<Unit>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _logger;
        public TrackUserInteractionCommandHandler(IRecommendationRepository recommendationRepository, ILoggerManager logger, IUserRepository userRepository)
        {
            _recommendationRepository = recommendationRepository;
            _logger = logger;
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<Unit>> Handle(TrackUserInteractionCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Tracking interaction of type {request.InteractionType} for User ID {request.UserId}");

            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {request.UserId} not found");
                return Errors.User.UserNotFound;
            }
            var interaction = new UserInteraction
            {
                UserId = request.UserId,
                Type = request.InteractionType,
                EventId = request.EventId,
                BlindDateId = request.BlindDateId,
                SpeedDatingEventId = request.SpeedDatingEventId,
                InteractedWithUserId = request.InteractedWithUserId,
                InteractionTime = DateTime.UtcNow,
                InteractionStrength = request.InteractionStrength
            };

            await _recommendationRepository.SaveUserInteractionAsync(interaction);
            _logger.LogInfo($"Successfully saved user interaction for user {request.UserId}");

            return Unit.Value;
        }
    }
}
