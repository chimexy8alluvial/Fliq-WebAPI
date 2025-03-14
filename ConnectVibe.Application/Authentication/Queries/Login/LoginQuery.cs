using Fliq.Application.Authentication.Common;
using Fliq.Application.Authentication.Queries.Login;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Security;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MediatR;


namespace Fliq.Application.Authentication.Queries.Login
{
    public record LoginQuery(
    string Email,
    string Password
    ) : IRequest<ErrorOr<AuthenticationResult>>;
}
public class LoginQueryHandler : IRequestHandler<LoginQuery, ErrorOr<AuthenticationResult>>
{
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserRepository _userRepository;
    private readonly ILoggerManager _logger;
    public LoginQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, ILoggerManager logger)
    {
        _jwtTokenGenerator = jwtTokenGenerator;
        _userRepository = userRepository;
        _logger = logger;
    }
    public async Task<ErrorOr<AuthenticationResult>> Handle(LoginQuery query, CancellationToken cancellationToken)
    {
        await Task.CompletedTask;
        var user = _userRepository.GetUserByEmail(query.Email);
        if (user == null)
            return Errors.Authentication.InvalidCredentials;

        var isSuccessfull = PasswordHash.Validate(query.Password, user.PasswordSalt, user.PasswordHash);
        _logger.LogInfo($"Validate user Query Result: {isSuccessfull}");
        if (!isSuccessfull)
            return Errors.Authentication.InvalidCredentials;


        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token, "");
    }

}
