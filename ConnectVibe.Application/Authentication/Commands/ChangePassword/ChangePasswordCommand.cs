﻿using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;
using Newtonsoft.Json;


namespace ConnectVibe.Application.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string Email,
        string OldPassword,
        string NewPassword
        ) : IRequest<ErrorOr<bool>>;

    public class ChangePasswordQueryHandler : IRequestHandler<ChangePasswordCommand, ErrorOr<bool>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        public ChangePasswordQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository, IOtpService otpService, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _otpService = otpService;
            _logger = logger;
        }
        public async Task<ErrorOr<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);
            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            var isSuccessfull = PasswordHash.Validate(command.OldPassword, user.PasswordSalt, user.PasswordHash);
            _logger.LogInfo($"Change Password command validation Result{JsonConvert.SerializeObject(isSuccessfull)}");
            if (!isSuccessfull)
                return Errors.Authentication.InvalidCredentials;

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.NewPassword, user.PasswordSalt);
            _userRepository.Update(user);

            await _emailService.SendEmailAsync(command.Email, "Password Changed", $"Successfully changed Password!");
            return true;
        }
    }
}
