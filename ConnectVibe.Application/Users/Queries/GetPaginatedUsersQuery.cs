using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Users.Common;
using Fliq.Application.Common.Pagination;

namespace Fliq.Application.Users.Queries
{
    public record GetPaginatedUsersQuery(int AdminUserId, int RoleId, int PageNumber, int PageSize) : IRequest<ErrorOr<PaginationResponse<UsersTableListResult>>>;

    public class GetPaginatedUsersQueryHandler : IRequestHandler<GetPaginatedUsersQuery, ErrorOr<PaginationResponse<UsersTableListResult>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private const int MaxUsersPerPage = 1000; // Limit to prevent large loads


        public GetPaginatedUsersQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<PaginationResponse<UsersTableListResult>>> Handle(GetPaginatedUsersQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Exporting users with Role ID {query.RoleId} requested by Admin ID {query.AdminUserId}");

            // Validate Admin User
            var adminUser = _userRepository.GetUserById(query.AdminUserId);
            if (adminUser == null)
            {
                _logger.LogError($"Admin user with ID {query.AdminUserId} not found.");
                return Errors.User.UserNotFound;
            }
            if (adminUser.RoleId == 3) // Regular users (RoleId = 3) cannot export
            {
                _logger.LogError($"User with ID {query.AdminUserId} is not authorized to export users.");
                return Errors.User.UnauthorizedUser;
            }
            if (adminUser.RoleId == 2 && query.RoleId != 3) // Admins (RoleId = 2) can only fetch Users (RoleId = 3)
            {
                _logger.LogError($"Admin with ID {query.AdminUserId} cannot export role {query.RoleId}.");
                return Errors.User.UnauthorizedUser;
            }

            // Enforce max page size
            var pageSize = Math.Min(query.PageSize, MaxUsersPerPage);

            // Fetch paginated users
            var users = await _userRepository.GetAllUsersByRoleIdAsync(query.RoleId, query.PageNumber, pageSize);
            if (!users.Any())
            {
                return Errors.User.UserNotFound;
            }

            return new PaginationResponse<UsersTableListResult>(users, users.Count(), query.PageNumber, pageSize);
        }

    }

}
