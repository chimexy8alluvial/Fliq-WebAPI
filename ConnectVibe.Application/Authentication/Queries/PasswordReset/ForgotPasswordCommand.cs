using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Authentication.Queries.Login;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MediatR;


namespace ConnectVibe.Application.Authentication.Queries.Login
{
    public record ForgotPasswordCommand(
    string Password,
    string ConfirmPassword,
    string Email,
    string Token
    ) : IRequest<ErrorOr<AuthenticationResult>>;
}
public class ResetPasswordHandler : IRequestHandler<ForgotPasswordCommand, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly IOtpService _otpService;
    public ResetPasswordHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IOtpService otpService)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _otpService = otpService;
    }
    public async Task<ErrorOr<AuthenticationResult>> Handle(ForgotPasswordCommand query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var user = _userRepository.GetUserByEmail(query.Email);
        if (user == null || !user.IsEmailValidated)
            return Errors.Authentication.InvalidCredentials;

        var otp = _otpService.GetOtpAsync(user.Email, user.Id);

        await _emailService.SendEmailAsync(command.Email, "Your OTP Code", $"Your OTP is {otp}");
        return new AuthenticationResult(otp);
    }

}
