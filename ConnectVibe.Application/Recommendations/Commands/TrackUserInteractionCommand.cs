using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities.Recommendations;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.Recommendations.Commands
{
    public record TrackUserInteractionCommand(
     int UserId,
     InteractionType Type,
     int? EventId,
     int? BlindDateId,
     int? SpeedDatingEventId,
     int? InteractedWithUserId,
     double InteractionStrength
        ) : IRequest<ErrorOr<Unit>>;

    public class TrackUserInteractionCommandHandler : IRequestHandler<TrackUserInteractionCommand, ErrorOr<Unit>>
    {
        private readonly IRecommendationRepository _recommendationRepository;
        private readonly ILoggerManager _loggerManager;
        public TrackUserInteractionCommandHandler(IRecommendationRepository recommendationRepository, ILoggerManager loggerManager)
        {
            _recommendationRepository = recommendationRepository;
            _loggerManager = loggerManager;
        }

        public async Task<ErrorOr<Unit>> Handle(TrackUserInteractionCommand request, CancellationToken cancellationToken)
        {
            var interaction = new UserInteraction
            {
                UserId = request.UserId,
                Type = request.Type,
                EventId = request.EventId,
                BlindDateId = request.BlindDateId,
                SpeedDatingEventId = request.SpeedDatingEventId,
                InteractedWithUserId = request.InteractedWithUserId,
                InteractionTime = DateTime.UtcNow,
                InteractionStrength = request.InteractionStrength
            };

            await _recommendationRepository.SaveUserInteractionAsync(interaction);

            return Unit.Value;
        }
    }
}
