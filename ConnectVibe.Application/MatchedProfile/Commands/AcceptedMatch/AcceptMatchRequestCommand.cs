using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.AcceptedMatch
{
    public class AcceptMatchRequestCommand : IRequest<ErrorOr<CreateAcceptMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
    }

    public class AcceptMatchRequestCommandHandler : IRequestHandler<AcceptMatchRequestCommand, ErrorOr<CreateAcceptMatchResult>>
    {

        private readonly IMatchProfileRepository _matchProfileRepository;

        public AcceptMatchRequestCommandHandler(IMatchProfileRepository matchProfileRepository)
        {

            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<CreateAcceptMatchResult>> Handle(AcceptMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var matchProfile = _matchProfileRepository.GetMatchProfileById(command.Id);
            if (matchProfile == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            matchProfile.matchRequestStatus = MatchRequestStatus.Accepted;
            _matchProfileRepository.Update(matchProfile);

            return new CreateAcceptMatchResult(matchProfile.MatchInitiatorUserId,
                 matchProfile.matchRequestStatus);
        }
    }
}
