using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;
namespace ConnectVibe.Application.Authentication.Commands.ValidatePasswordOTP
{
    public record ValidatePasswordOTPCommand(
       string Email,
        string Otp
    ) :  IRequest<ErrorOr<ValidatePasswordOTPResult>>;

    public  class ValidatePasswordOTPCommandHandler : IRequestHandler<ValidatePasswordOTPCommand, ErrorOr<ValidatePasswordOTPResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;

        public ValidatePasswordOTPCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpService otpService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
        }

        public async Task<ErrorOr<ValidatePasswordOTPResult>> Handle(ValidatePasswordOTPCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (!await _otpService.ValidateOtpAsync(command.Email, command.Otp))
                return Errors.Authentication.InvalidOTP;
            var response = true;
            return new ValidatePasswordOTPResult(response);
        }
    }
}
