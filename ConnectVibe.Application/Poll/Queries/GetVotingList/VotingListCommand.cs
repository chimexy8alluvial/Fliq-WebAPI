using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Poll.Common;
using Fliq.Contracts.Polls;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Poll.Queries.GetVotingList
{
    public record VotingListCommand(int UserId) : IRequest<ErrorOr<List<VotingListDto>>>;

    public class VotingListCommandHandler : IRequestHandler<VotingListCommand, ErrorOr<List<VotingListDto>>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IPollRepository _pollRepository;

        public VotingListCommandHandler(IMapper mapper, ILoggerManager logger, IPollRepository pollRepository)
        {
            _mapper = mapper;
            _logger = logger;
            _pollRepository = pollRepository;
        }
        public async Task<ErrorOr<List<VotingListDto>>> Handle(VotingListCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var pollResultList = await _pollRepository.GetVotingList(command.UserId);
            var result = pollResultList.ToList();

            return result;
        }
    }
}
