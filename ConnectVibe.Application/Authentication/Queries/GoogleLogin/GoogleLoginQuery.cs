using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.AuthServices;
using Fliq.Domain.Entities;
using ErrorOr;
using MediatR;
using Newtonsoft.Json;

namespace Fliq.Application.Authentication.Queries.GoogleLogin
{
    public record GoogleLoginQuery(
   string Code
   ) : IRequest<ErrorOr<SocialAuthenticationResult>>;

    public class GoogleLoginQueryHandler : IRequestHandler<GoogleLoginQuery, ErrorOr<SocialAuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ISocialAuthService _socialAuthService;
        private readonly ILoggerManager _logger;

        public GoogleLoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ISocialAuthService socialAuthService, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _socialAuthService = socialAuthService;
            _logger = logger;

        }

        public async Task<ErrorOr<SocialAuthenticationResult>> Handle(GoogleLoginQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            bool isNewUser = false;
            var googleResponse = await _socialAuthService.ExchangeCodeForTokenAsync(query.Code);
            _logger.LogInfo($"Exchange Code For Token Query Result: {googleResponse}");
            var user = _userRepository.GetUserByEmail(googleResponse.Email);
            _logger.LogInfo($"Get User by Email Query Result: {JsonConvert.SerializeObject(user)}");
            if (user == null)
            {
                user = new User()
                {
                    Email = googleResponse.Email,
                    FirstName = googleResponse.GivenName,
                    LastName = googleResponse.FamilyName,
                    DisplayName = googleResponse.Name,
                    IsEmailValidated = googleResponse.EmailVerified
                };
                _userRepository.Add(user);
                isNewUser = true;
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new SocialAuthenticationResult(user, token, isNewUser);
        }
    }
}
