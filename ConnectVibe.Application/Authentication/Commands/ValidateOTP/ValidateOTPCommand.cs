using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;
namespace ConnectVibe.Application.Authentication.Commands.ValidateOTP
{
    public record ValidateOTPCommand(
        string Email,
        string Otp
        ) : IRequest<ErrorOr<AuthenticationResult>>;


    public class ValidateOTPCommandHandler : IRequestHandler<ValidateOTPCommand, ErrorOr<AuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        public ValidateOTPCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
        }
        public async Task<ErrorOr<AuthenticationResult>> Handle(ValidateOTPCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            if (!_otpRepository.CheckActiveOtp(command.Email, command.Otp))
                return Errors.Authentication.InvalidOTP;

            var user = _userRepository.GetUserByEmail(command.Email);
            user.IsEmailValidated = true;
            _userRepository.Update(user);
            var token = _jwtTokenGenerator.GenerateToken(user);
            return new AuthenticationResult(user, token);
        }

    }

}
