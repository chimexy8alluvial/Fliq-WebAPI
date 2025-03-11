using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Application.DashBoard.Queries.FemaleUsersCount
{

    public record GetAllFemaleUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetAllFemaleUsersCountQueryHandler : IRequestHandler<GetAllFemaleUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllFemaleUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetAllFemaleUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all female-users count...");

            var count = await _userRepository.CountAllFemaleUsers();
            _logger.LogInfo($"All female-users count: {count}");

            return new UserCountResult(count);
        }
    }
}
