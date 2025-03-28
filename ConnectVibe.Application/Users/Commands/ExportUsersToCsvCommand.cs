using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;
using Quartz;

namespace Fliq.Application.Users.Commands
{
    public record ExportUsersToCsvCommand(int AdminUserId, int RoleId, int PageNumber, int PageSize)
      : IRequest<ErrorOr<Success>>;


    public class ExportUsersToCsvCommandHandler
    : IRequestHandler<ExportUsersToCsvCommand, ErrorOr<Success>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly ISchedulerFactory _schedulerFactory;

        public ExportUsersToCsvCommandHandler(
            IUserRepository userRepository,
            ILoggerManager logger, ISchedulerFactory schedulerFactory)
        {
            _userRepository = userRepository;
            _logger = logger;
            _schedulerFactory = schedulerFactory;
        }

        public async Task<ErrorOr<Success>> Handle(ExportUsersToCsvCommand command, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Exporting users for Role ID {command.RoleId}, Page {command.PageNumber}, Size {command.PageSize} by Admin {command.AdminUserId}");

            // Validate Admin User
            var adminUser =  _userRepository.GetUserById(command.AdminUserId);
            if (adminUser == null) return Errors.User.UserNotFound;
            if (adminUser.RoleId == 3) return Errors.User.UnauthorizedUser;
            if (adminUser.RoleId == 2 && command.RoleId != 3) return Errors.User.UnauthorizedUser;

            var scheduler = await _schedulerFactory.GetScheduler(cancellationToken);
            var jobKey = new JobKey("ExportUsersJob");

            if (await scheduler.CheckExists(jobKey, cancellationToken))
            {
                var jobData = new JobDataMap
            {
                { "adminUserId", command.AdminUserId },
                { "adminUserEmail", adminUser.Email },
                { "roleId", command.RoleId },
                { "pageNumber", command.PageNumber },
                { "pageSize", command.PageSize }
            };

                await scheduler.TriggerJob(jobKey, jobData, cancellationToken);
            }
            else
            {
                return Errors.Jobs.JobNotFound;
            }

            return new Success();
        }
    }
}
