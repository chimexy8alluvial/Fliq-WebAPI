using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Common;
using Fliq.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Settings.Queries.GetSettings
{
    public record GetSettingsQuery(int UserId) : IRequest<ErrorOr<GetSettingsResult>>;

    public class GetSettingsQueryHandler : IRequestHandler<GetSettingsQuery, ErrorOr<GetSettingsResult>>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILoggerManager _logger;

        public GetSettingsQueryHandler(ISettingsRepository settingsRepository, IProfileRepository profileRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor, ILoggerManager logger)
        {
            _settingsRepository = settingsRepository;
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        public async Task<ErrorOr<GetSettingsResult>> Handle(GetSettingsQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User: {request.UserId} not found");
                return Errors.User.UserNotFound;
            }

            var settings = _settingsRepository.GetSettingByUserId(request.UserId);
            if (settings == null)
            {
                _logger.LogError($"Settings not found");
                return Errors.Settings.SettingsNotFound;
            }

            return new GetSettingsResult(
              settings.Id,
              settings.ScreenMode,
              settings.RelationAvailability,
              settings.ShowMusicAndGameStatus,
              settings.Language,
              settings.NotificationPreferences.ToList(),
              settings.Filter,
              user.FirstName + " " + user.LastName,
              user.Email,
              user.Id
              );
        }
    }
}