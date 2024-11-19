using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.PaymentServices;
using Fliq.Application.Common.Interfaces.Services.SubscriptionServices;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Poll.Common;
using MapsterMapper;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public CreatePollCommandHandler(IUserRepository userRepository, IMapper mapper, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ErrorOr<CreatePollResult>> Handle(CreatePollCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
