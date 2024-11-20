using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Poll.Common;
using Fliq.Domain.Entities.VotingPoll;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Poll.Commands.Voting
{
    public class VoteCommand : IRequest<ErrorOr<VoteResult>>
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Question { get; set; } = default!;
        public List<string> Options { get; set; } = default!;
    }

    public class VoteCommandHandler : IRequestHandler<VoteCommand, ErrorOr<VoteResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IPollRepository _pollRepository;

        public VoteCommandHandler(IMapper mapper, ILoggerManager logger, IPollRepository pollRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _pollRepository = pollRepository;
        }
        public async Task<ErrorOr<VoteResult>> Handle(VoteCommand command, CancellationToken cancellationToken)
        {
            var initiateVote = _mapper.Map<VotePoll>(command);
            initiateVote.Count = +1;
            _pollRepository.Vote(initiateVote);

            return new VoteResult
            {
                SuccessStatus = true,
                Message = $"Vote was successful for User: {command.UserId}"
            };
        }
    }

}
