
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;
using System.Text;
using Fliq.Domain.Common.Errors;
using Fliq.Application.Users.Common;

namespace Fliq.Application.Users.Queries
{
    public record ExportUsersToCsvQuery(int AdminUserId, int RoleId) : IRequest<ErrorOr<byte[]>>;

    public class ExportUsersToCsvQueryHandler : IRequestHandler<ExportUsersToCsvQuery, ErrorOr<byte[]>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public ExportUsersToCsvQueryHandler(IUserRepository userRepository, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<byte[]>> Handle(ExportUsersToCsvQuery query, CancellationToken cancellationToken)
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

            // Fetch users based on role
            var users = await _userRepository.GetAllUsersByRoleIdAsync(query.RoleId);
            if (!users.Any())
            {
                return Errors.User.UserNotFound;
            }

            // Generate CSV
            var csvData = GenerateCsv(users);
            return csvData;
        }

        private byte[] GenerateCsv(IEnumerable<UsersTableListResult> users)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Subscription,Date Joined,Last Active");

            foreach (var user in users)
            {
                var FullName = $"{user.FirstName} { user.LastName}";
                sb.AppendLine($"{FullName},{user.Email},{user.Subscription},{user.DateCreated:yyyy-MM-dd},{user.LastActiveAt:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }

}
