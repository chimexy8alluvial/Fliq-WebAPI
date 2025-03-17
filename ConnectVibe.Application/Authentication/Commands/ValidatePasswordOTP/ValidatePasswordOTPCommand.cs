using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MediatR;
namespace Fliq.Application.Authentication.Commands.ValidatePasswordOTP
{
    public record ValidatePasswordOTPCommand(
       string Email,
        string Otp
    ) : IRequest<ErrorOr<ValidatePasswordOTPResult>>;

    public class ValidatePasswordOTPCommandHandler : IRequestHandler<ValidatePasswordOTPCommand, ErrorOr<ValidatePasswordOTPResult>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOtpService _otpService;

        public ValidatePasswordOTPCommandHandler(IUserRepository userRepository, IOtpService otpService)
        {
            _userRepository = userRepository;
            _otpService = otpService;
        }

        public async Task<ErrorOr<ValidatePasswordOTPResult>> Handle(ValidatePasswordOTPCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (!await _otpService.ValidateOtpAsync(command.Email, command.Otp))
                return Errors.Authentication.InvalidOTP;
            var user = _userRepository.GetUserByEmail(command.Email);
            return new ValidatePasswordOTPResult(user,command.Otp);
        }
    }
}
