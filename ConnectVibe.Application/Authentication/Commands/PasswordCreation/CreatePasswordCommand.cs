using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MediatR;
using Newtonsoft.Json;

namespace ConnectVibe.Application.Authentication.Commands.PasswordCreation
{
    public record CreatePasswordCommand(
        int Id,
        string ConfirmPassword,
        string NewPassword,
        string Otp
        ) : IRequest<ErrorOr<bool>>;

    public class CreatePasswordHandler : IRequestHandler<CreatePasswordCommand, ErrorOr<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        public CreatePasswordHandler(IUserRepository userRepository, IEmailService emailService, IOtpService otpService, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
        }
        public async Task<ErrorOr<bool>> Handle(CreatePasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserById(command.Id);
            _logger.LogInfo($"Create Password command validation Result: {user}");
            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            if (!await _otpService.OtpExistAsync(user.Email, command.Otp))
                return Errors.Authentication.InvalidCredentials;


            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.NewPassword, user.PasswordSalt);
            _userRepository.Update(user);
            await _emailService.SendEmailAsync(user.Email, "Password Creation", $"Successfully created Password!");
            return true;

        }
    }
}
