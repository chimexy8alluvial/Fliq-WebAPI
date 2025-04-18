﻿using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.AuthServices;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using ErrorOr;
using MediatR;

namespace Fliq.Application.Authentication.Queries.FacebookLogin
{
    public record FacebookLoginQuery(
    string Code
    ) : IRequest<ErrorOr<SocialAuthenticationResult>>;

    public class FacebookLoginQueryHandler : IRequestHandler<FacebookLoginQuery, ErrorOr<SocialAuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly ISocialAuthService _socialAuthService;
        private readonly ILoggerManager _logger;

        public FacebookLoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ISocialAuthService socialAuthService, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _socialAuthService = socialAuthService;
            _logger = logger;
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
