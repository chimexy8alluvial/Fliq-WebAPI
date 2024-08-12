using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Authentication.Queries.FacebookLogin
{
    public record FacebookLoginQuery(
    string Code
    ) : IRequest<ErrorOr<SocialAuthenticationResult>>;

    public class FacebookLoginQueryHandler : IRequestHandler<FacebookLoginQuery, ErrorOr<SocialAuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ISocialAuthService _socialAuthService;

        public FacebookLoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ISocialAuthService socialAuthService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _socialAuthService = socialAuthService;
        }

        public async Task<ErrorOr<SocialAuthenticationResult>> Handle(FacebookLoginQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            bool isNewUser = false;
            var facebookResponse = await _socialAuthService.GetFacebookUserInformation(query.Code);
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
                    DisplayName = facebookResponse.Name,
                    IsEmailValidated = true
                };
                _userRepository.Add(user);
                isNewUser = true;
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new SocialAuthenticationResult(user, token, isNewUser);
        }
    }
}