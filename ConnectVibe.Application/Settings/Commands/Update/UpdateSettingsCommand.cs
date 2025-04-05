using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Settings.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Fliq.Application.Settings.Commands.Update
{
    public record UpdateSettingsCommand
        (
        int Id,
        ScreenMode ScreenMode,
         bool RelationAvailability,
        bool ShowMusicAndGameStatus,
        Language Language,
        List<NotificationPreference> NotificationPreferences,
        Filter Filter,
        int UserId
        ) : IRequest<ErrorOr<GetSettingsResult>>;

    public class UpdateSettingsCommandHandler : IRequestHandler<UpdateSettingsCommand, ErrorOr<GetSettingsResult>>
    {
        private readonly ISettingsRepository _settingsRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateSettingsCommandHandler(ISettingsRepository settingsRepository, IUserRepository userRepository, ILoggerManager logger, IHttpContextAccessor httpContextAccessor)
        {
            _settingsRepository = settingsRepository;
            _userRepository = userRepository;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ErrorOr<GetSettingsResult>> Handle(UpdateSettingsCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(command.UserId);
            if (user == null)
            {
                _logger.LogError($"User with Id: {command.UserId}, not found");
                return Errors.User.UserNotFound;
            }

            var settings = _settingsRepository.GetSettingById(command.Id);
            if (settings == null)
            {
                _logger.LogError($"Setting {command.Id} not found");
                return Errors.Settings.SettingsNotFound;
            }

            var updatedSettings = command.Adapt(settings);

            _settingsRepository.Update(updatedSettings);
            _logger.LogInfo($"Settings updated for user {user.Id}");

            return new GetSettingsResult(
                updatedSettings.Id,
                updatedSettings.ScreenMode,
                updatedSettings.RelationAvailability,
                updatedSettings.ShowMusicAndGameStatus,
                updatedSettings.Language,
                updatedSettings.NotificationPreferences.ToList(),
                updatedSettings.Filter,
                user.FirstName + " " + user.LastName,
                user.Email,
                user.Id
                );
        }
    }
}