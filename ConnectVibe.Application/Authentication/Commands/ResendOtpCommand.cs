

using ErrorOr;
using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using MediatR;

namespace Fliq.Application.Authentication.Commands
{
    public record ResendOtpCommand(
   string Email ) : IRequest<ErrorOr<RegistrationResult>>;

    public class ResendOtpCommandHandler : IRequestHandler<ResendOtpCommand, ErrorOr<RegistrationResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        public ResendOtpCommandHandler(IUserRepository userRepository, IEmailService emailService, IOtpService otpService, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
        }

        public async Task<ErrorOr<RegistrationResult>> Handle(ResendOtpCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            User? user = _userRepository.GetUserByEmail(command.Email);
            _logger.LogInfo($"Resend otp command executed for: {user}");
         
            if (user == null)
                return Errors.User.UserNotFound;

            var otp = await _otpService.GetOtpAsync(user.Email, user.Id);
            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
            return new RegistrationResult(user, otp);
        }
    }
}
