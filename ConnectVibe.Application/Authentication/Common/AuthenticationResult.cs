using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Authentication.Common;

public record AuthenticationResult
   (
       User user,
       string Token
    );
