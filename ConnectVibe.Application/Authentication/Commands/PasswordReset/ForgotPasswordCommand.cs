using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MediatR;


namespace Fliq.Application.Authentication.Commands.PasswordReset
{
    public record ForgotPasswordCommand(
      string Email
    ) : IRequest<ErrorOr<ForgotPasswordResult>>;

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ErrorOr<ForgotPasswordResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILoggerManager _logger;
        public ForgotPasswordHandler(IUserRepository userRepository, IOtpService otpService, IEmailService emailService, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<ErrorOr<ForgotPasswordResult>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);
            _logger.LogInfo($"Forgot Password validation command: ----{user}");

            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            var otp =await _otpService.GetOtpAsync(user.Email, user.Id);
            _logger.LogInfo($"{user.Email} recieved the following otp--{otp}");

            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
            return new ForgotPasswordResult(otp,user.Email);
        }

    }
}

