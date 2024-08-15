using ConnectVibe.Application.Authentication.Commands.PasswordReset;
using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MediatR;


namespace ConnectVibe.Application.Authentication.Commands.PasswordReset
{
    public record ForgotPasswordCommand(
      string Email
    ) : IRequest<ErrorOr<ForgotPasswordResult>>;

    public class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand, ErrorOr<ForgotPasswordResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;
        private readonly IEmailService _emailService;
        private readonly ILoggerManager _logger;
        public ForgotPasswordHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IOtpService otpService, IEmailService emailService, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _otpService = otpService;
            _emailService = emailService;
            _logger = logger;
        }
        public async Task<ErrorOr<ForgotPasswordResult>> Handle(ForgotPasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);

            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            var otp =await _otpService.GetOtpAsync(user.Email, user.Id);
            _logger.LogInfo($"{user.Email} recieved the following otp--{otp}");

            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
            return new ForgotPasswordResult(otp,user.Email);
        }

    }
}

