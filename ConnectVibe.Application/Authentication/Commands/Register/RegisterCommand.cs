using ErrorOr;
using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Security;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities;
using Fliq.Domain.Entities.Settings;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.Authentication.Commands.Register
{
    public record RegisterCommand(
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Password,
    Language Language,
    ScreenMode Theme,
    string PhoneNumber
    ) : IRequest<ErrorOr<RegistrationResult>>;

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<RegistrationResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        private readonly ISettingsRepository _settingsRepository;

        public RegisterCommandHandler(IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpService otpService, ILoggerManager logger, ISettingsRepository settingsRepository)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
            _settingsRepository = settingsRepository;
        }

        public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            User? user = _userRepository.GetUserByEmail(command.Email);
            _logger.LogInfo($"Register command validation Result: {user}");
            if (user != null && user.IsEmailValidated)
                return Errors.User.DuplicateEmail;

            if (user == null)
                user = _mapper.Map<User>(command);

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.Password, user.PasswordSalt);
            user.RoleId = 3;
            _userRepository.Add(user);

            Setting setting = new Setting { ScreenMode = command.Theme, Language = command.Language, User = user, UserId = user.Id };
            _settingsRepository.Add(setting);

            var otp = await _otpService.GetOtpAsync(user.Email, user.Id);
            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
            return new RegistrationResult(user, otp);
        }
    }
}