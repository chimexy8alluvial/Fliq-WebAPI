using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using MapsterMapper;
using MediatR;


namespace Fliq.Application.DashBoard.Queries.GetAllUser
{
    public record GetAllUsersQuery(int PageNumber, int PageSize) : IRequest<ErrorOr<List<CreateUserResult>>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<List<CreateUserResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public GetAllUsersQueryHandler(IUserRepository userRepository, IMapper mapper, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ErrorOr<List<CreateUserResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Getting users for page {query.PageNumber} with page size {query.PageSize}");


            var users = _userRepository.GetAllUsersForDashBoard(query.PageNumber, query.PageSize);


            var userList = users.ToList();

            _logger.LogInfo($"Got {userList.Count} users for page {query.PageNumber}");


            var results = users.Select(user =>
            {
                
                var latestSubscription = user.Subscriptions?
                    .OrderByDescending(s => s.StartDate)
                    .FirstOrDefault();

                return new CreateUserResult(
                    DisplayName: user.DisplayName,
                    Email: user.Email,
                    SubscriptionType: latestSubscription?.ProductId ?? "None",
                    DateJoined: user.DateCreated,
                    LastOnline: user.LastActiveAt
                );
            }).ToList();

            return results;
        }
    }
}
