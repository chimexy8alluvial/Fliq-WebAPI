using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ConnectVibe.Domain.Entities;
using ErrorOr;
using MapsterMapper;
using MediatR;
using Newtonsoft.Json;


namespace ConnectVibe.Application.Authentication.Commands.Register
{
    public record RegisterCommand(
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Password
    ) : IRequest<ErrorOr<RegistrationResult>>;



    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, ErrorOr<RegistrationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository, IOtpService otpService, ILoggerManager logger)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _otpService = otpService;
            _logger = logger;
        }
        public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            User user = _userRepository.GetUserByEmail(command.Email);
            _logger.LogInfo($"------Register command: ----{JsonConvert.SerializeObject(user)}");
            if (user != null && user.IsEmailValidated)
                return Errors.User.DuplicateEmail;

            if (user == null)
                user = _mapper.Map<User>(command);

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.Password, user.PasswordSalt);
            _userRepository.Add(user);

            var otp = await _otpService.GetOtpAsync(user.Email, user.Id);
            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
            return new RegistrationResult(user, otp);
        }

    }
}
