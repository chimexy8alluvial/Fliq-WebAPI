using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Poll.Common;
using Fliq.Domain.Entities.VotingPoll;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Poll.Commands.CreatePoll
{
    public class CreatePollCommand : IRequest<ErrorOr<CreatePollResult>>
    {
        public int UserId { get; set; }
        public int EventId { get; set; }
        public string Question { get; set; } = default!;
        public List<string> Options { get; set; } = default!;
        public bool multipleOptionSelect { get; set; }
    }

    public class CreatePollCommandHandler : IRequestHandler<CreatePollCommand, ErrorOr<CreatePollResult>>
    {
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        private readonly IPollRepository _pollRepository;

        public CreatePollCommandHandler(IMapper mapper, ILoggerManager logger, IPollRepository pollRepository)
        {
            
            _mapper = mapper;
            _logger = logger;
            _pollRepository = pollRepository;
        }

        public async Task<ErrorOr<CreatePollResult>> Handle(CreatePollCommand command, CancellationToken cancellationToken)
        {
            var initiatePoll= _mapper.Map<VotePoll>(command);
            initiatePoll.DateCreated = DateTime.Now;
            _pollRepository.CreateVote(initiatePoll);

            return new CreatePollResult
            {
                SuccessStatus = true,
                Message = $"Poll creation was successful for User: {command.UserId}"
            };

        }
    }
}
