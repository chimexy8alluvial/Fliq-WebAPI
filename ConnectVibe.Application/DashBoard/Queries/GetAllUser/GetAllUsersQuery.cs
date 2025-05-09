using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using MediatR;


namespace Fliq.Application.DashBoard.Queries.GetAllUser
{
    public record GetAllUsersQuery(PaginationRequest PaginationRequest,
                                        bool? HasSubscription=null ,
                                          DateTime? ActiveSince = null,
                                            string? RoleName = null)          : IRequest<ErrorOr<List<GetUsersResult>>>;

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, ErrorOr<List<GetUsersResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetAllUsersQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;          
            _logger = logger;
        }

        public async Task<ErrorOr<List<GetUsersResult>>> Handle(GetAllUsersQuery query, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInfo($"Getting users for page {query.PaginationRequest.PageNumber} with page size {query.PaginationRequest.PageSize}");

                if (query.PaginationRequest == null)
                {
                    _logger.LogWarn("PaginationRequest is null, using default values");
                    query = query with { PaginationRequest = new PaginationRequest() };
                }

                // Validate pagination parameters
                if (query.PaginationRequest.PageNumber < 1)
                    return Error.Validation("InvalidPagination", "Page number must be greater than 0");
                if (query.PaginationRequest.PageSize < 1)
                    return Error.Validation("InvalidPagination", "Page size must be greater than 0");

                var request = new GetUsersListRequest
                {
                    PaginationRequest = query.PaginationRequest,
                    HasSubscription = query.HasSubscription,
                    ActiveSince = query.ActiveSince,
                    RoleName = query.RoleName
                };

                var users = await _userRepository.GetAllUsersForDashBoardAsync(request);

                if (users == null)
                    return Error.Failure("DatabaseError", "Failed to retrieve users from database");

                _logger.LogInfo($"Got {users.Count()} users for page {query.PaginationRequest.PageNumber}");

                return users.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error retrieving users: {ex.Message}");
                return Error.Failure("UnexpectedError", $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
