using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using ConnectVibe.Domain.Entities;
using ErrorOr;
using MediatR;

namespace ConnectVibe.Application.Authentication.Queries.GoogleLogin
{
    public record GoogleLoginQuery(
   string Code
   ) : IRequest<ErrorOr<SocialAuthenticationResult>>;

    public class GoogleLoginQueryHandler : IRequestHandler<GoogleLoginQuery, ErrorOr<SocialAuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ISocialAuthService _socialAuthService;

        public GoogleLoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ISocialAuthService socialAuthService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _socialAuthService = socialAuthService;
        }

        public async Task<ErrorOr<SocialAuthenticationResult>> Handle(GoogleLoginQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            bool isNewUser = false;
            var googleResponse = await _socialAuthService.ExchangeCodeForTokenAsync(query.Code);
            var user = _userRepository.GetUserByEmail(googleResponse.Email);
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