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

namespace Fliq.Application.DashBoard.Queries.MaleUsersCount
{
    public record GetAllMaleUsersCountQuery() : IRequest<ErrorOr<UserCountResult>>;

    public class GetAllMaleUsersCountQueryHandler : IRequestHandler<GetAllMaleUsersCountQuery, ErrorOr<UserCountResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllMaleUsersCountQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<UserCountResult>> Handle(GetAllMaleUsersCountQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo("Fetching all male-users count...");

            var count = await _userRepository.CountAllMaleUsers();
            _logger.LogInfo($"All male-users count: {count}");

            return new UserCountResult(count);
        }
    }
}
