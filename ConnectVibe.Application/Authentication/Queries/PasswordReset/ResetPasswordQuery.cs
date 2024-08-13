using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Authentication.Queries.Login;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MediatR;


namespace ConnectVibe.Application.Authentication.Queries.Login
{
    public record ResetPasswordQuery(
    string Password,
    string ConfirmPassword,
    string Email,
    string Token
    ) : IRequest<ErrorOr<AuthenticationResult>>;
}
public class ResetPasswordHandler : IRequestHandler<ResetPasswordQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    public ResetPasswordHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
    }
    public async Task<ErrorOr<AuthenticationResult>> Handle(ResetPasswordQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var user = _userRepository.GetUserByEmail(query.Email);
        if (user == null)
            return Errors.Authentication.InvalidCredentials;

       


        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }

}
