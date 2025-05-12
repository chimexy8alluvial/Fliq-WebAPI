using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.AuthServices;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using ErrorOr;
using MediatR;
using Fliq.Domain.Enums;
using Fliq.Domain.Entities.Settings;

namespace Fliq.Application.Authentication.Queries.FacebookLogin
{
    public record FacebookLoginQuery(
    string Code, string? DisplayName, Language Language, string Theme
    ) : IRequest<ErrorOr<SocialAuthenticationResult>>;

    public class FacebookLoginQueryHandler : IRequestHandler<FacebookLoginQuery, ErrorOr<SocialAuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ISocialAuthService _socialAuthService;
        private readonly ILoggerManager _logger;
        private readonly ISettingsRepository _settingsRepository;

        public FacebookLoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ISocialAuthService socialAuthService, ILoggerManager logger, ISettingsRepository settingsRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _socialAuthService = socialAuthService;
            _logger = logger;
            _settingsRepository = settingsRepository;
        }

        public async Task<ErrorOr<SocialAuthenticationResult>> Handle(FacebookLoginQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            bool isNewUser = false;
            var facebookResponse = await _socialAuthService.GetFacebookUserInformation(query.Code);
            _logger.LogInfo($"Get Facebook User Information Query Result: {facebookResponse}");
            if (facebookResponse == null)
            {
                return Errors.Authentication.InvalidToken;
            }
            var user = _userRepository.GetUserByEmail(facebookResponse.Email);
            if (user == null)
            {
                user = new User()
                {
                    Email = facebookResponse.Email,
                    FirstName = facebookResponse.FirstName,
                    LastName = facebookResponse.LastName,
                    DisplayName = query.DisplayName,
                    IsEmailValidated = true
                };
                _userRepository.Add(user);
                isNewUser = true;
                Setting setting = new Setting { ScreenMode = query.Theme, Language = query.Language, User = user, UserId = user.Id };
                _settingsRepository.Add(setting);
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new SocialAuthenticationResult(user, token, isNewUser);
        }
    }
}