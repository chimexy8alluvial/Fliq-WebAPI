﻿using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MediatR;
using StreamChat.Models;
using StreamChat.Clients;
using StreamChat.Exceptions;

namespace Fliq.Application.Authentication.Commands.ValidateOTP
{
    public record ValidateOTPCommand(
        string Email,
        string Otp
        ) : IRequest<ErrorOr<AuthenticationResult>>;

    public class ValidateOTPCommandHandler : IRequestHandler<ValidateOTPCommand, ErrorOr<AuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IStreamClientFactory _streamClientFactory;
        private readonly ILoggerManager _logger;

        public ValidateOTPCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IOtpService otpService, IStreamClientFactory streamClientFactory, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _otpService = otpService;
            _streamClientFactory = streamClientFactory;
            _logger = logger;
        }

        public async Task<ErrorOr<AuthenticationResult>> Handle(ValidateOTPCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (!await _otpService.ValidateOtpAsync(command.Email, command.Otp))
                return Errors.Authentication.InvalidOTP;
            var user = _userRepository.GetUserByEmail(command.Email);
            user.IsEmailValidated = true;
            user.IsActive = true;
            _userRepository.Update(user);
            var token = _jwtTokenGenerator.GenerateToken(user);

            #region Stream Api implementation
            // Generate Stream Chat token
            var userClient = _streamClientFactory.GetUserClient();
            var streamToken = userClient.CreateToken(user.Id.ToString()); // Use user ID or username

            //sync user with stream
            var streamUser = new UserRequest
            {
                Id = user.Id.ToString(),
                Role = user.Role?.Name.ToLower() == "user" ? "user" : "admin",
            };

            streamUser.SetData("Email", user.Email);
            streamUser.SetData("Name", user.FirstName + user.LastName);

            try
            {
                await userClient.UpsertManyAsync([streamUser]);
            }
            catch (StreamChatException ex)
            {
                // Log the error but don't fail authentication
                _logger.LogWarn($"Failed to sync user with Stream Chat, ex: {ex}");
                // Continue with authentication anyway
            }
            #endregion

            return new AuthenticationResult(user, token, streamToken);
        }
    }
}