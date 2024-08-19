using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Authentication.Queries.Login;
using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MediatR;
using Newtonsoft.Json;


namespace ConnectVibe.Application.Authentication.Queries.Login
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
        _logger.LogInfo($"-----LoginQuery Response: ----{JsonConvert.SerializeObject(user)}");
        if (user == null)
            return Errors.Authentication.InvalidCredentials;

        var isSuccessfull = PasswordHash.Validate(query.Password, user.PasswordSalt, user.PasswordHash);
        _logger.LogInfo($"-----LoginQuery isSuccessful Response: ----{JsonConvert.SerializeObject(isSuccessfull)}");
        if (!isSuccessfull)
            return Errors.Authentication.InvalidCredentials;


        var token = _jwtTokenGenerator.GenerateToken(user);

        return new AuthenticationResult(user, token);
    }

}
