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
        public RegisterCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository, IOtpService otpService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _otpService = otpService;
        }
        public async Task<ErrorOr<RegistrationResult>> Handle(RegisterCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            User user = _userRepository.GetUserByEmail(command.Email);

            if (user != null && user.IsEmailValidated)
                return Errors.User.DuplicateEmail;

            if (user == null)
                user = _mapper.Map<User>(command);

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.Password, user.PasswordSalt);
            _userRepository.Add(user);

            var otp = new OTP { Code = _otpService.GenerateOtp(), Email = command.Email, ExpiresAt = DateTime.UtcNow.AddMinutes(10), UserId = user.Id };
            _otpRepository.Add(otp);

            await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp.Code}");
            return new RegistrationResult(user, otp.Code);
        }

    }
}
