using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.MeidaServices;
using Fliq.Application.Users.Common;
using Quartz;
using System.Text;

namespace Fliq.Application.SchedulingServices.QuartzJobs
{
    public class ExportUsersJob : IJob
    {
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        private IMediaServices _mediaServices;
        const int MAX_EXPORT_SIZE = 10000;

        public ExportUsersJob(ILoggerManager logger, IUserRepository userRepository, IMediaServices mediaServices)
        {
            _logger = logger;
            _userRepository = userRepository;
            _mediaServices = mediaServices;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInfo("Starting User Export Job...");

            try
            {
                JobDataMap dataMap = context.MergedJobDataMap;
                int adminUserId = dataMap.GetInt("adminUserId");
                string adminUserEmail = dataMap.GetString("adminUserEmail")!;
                int roleId = dataMap.GetInt("roleId");
                int pageNumber = dataMap.GetInt("pageNumber");
                int pageSize = dataMap.GetInt("pageSize");

                // Enforce max export size
                pageSize = Math.Min(pageSize, MAX_EXPORT_SIZE);

                // Fetch paginated users
                var users = await _userRepository.GetAllUsersByRoleIdAsync( roleId, pageNumber, pageSize);
                if (!users.Any())
                {
                    _logger.LogWarn("No users found for export.");
                    return;
                }

                // Generate CSV file
                string fileName = $"fliq_users_page_{pageNumber}_size_{pageSize}_{DateTime.UtcNow:yyyyMMddHHmmss}.csv";
                string filePath = Path.Combine(Path.GetTempPath(), fileName);

                // Upload file to Azure
                byte[] csvData = GenerateCsv(users);
                string? fileUrl = await _mediaServices.UploadDocumentAsync(csvData, fileName, "exports");


                if (!string.IsNullOrEmpty(fileUrl))
                {
                    _logger.LogInfo($"User export successful! File URL: {fileUrl}");
                    //send email to user here
                }
                else
                {
                    _logger.LogError("User export failed: Unable to upload file.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in ExportUsersJob: {ex.Message}");
            }
        }

        private byte[] GenerateCsv(IEnumerable<UsersTableListResult> users)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Name,Email,Subscription,Date Joined,Last Active");

            foreach (var user in users)
            {
                var FullName = $"{user.FirstName} {user.LastName}";
                sb.AppendLine($"{FullName},{user.Email},{user.Subscription},{user.DateCreated:yyyy-MM-dd},{user.LastActiveAt:yyyy-MM-dd}");
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }
    }
}
