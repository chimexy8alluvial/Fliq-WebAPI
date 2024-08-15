using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;

namespace ConnectVibe.Application.Authentication.Commands.PasswordCreation
{
    public record CreatePasswordCommand(
        string Email,
        string ConfirmPassword,
        string NewPassword
        ) : IRequest<ErrorOr<bool>>;

    public class CreatePasswordHandler : IRequestHandler<CreatePasswordCommand, ErrorOr<bool>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        public CreatePasswordHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository, IOtpService otpService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _otpService = otpService;
        }
        public async Task<ErrorOr<bool>> Handle(CreatePasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);
            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.NewPassword, user.PasswordSalt);
            _userRepository.Update(user);

            await _emailService.SendEmailAsync(command.Email, "Password Creation", $"Successfully created Password!");
            return true;

        }
    }
}
