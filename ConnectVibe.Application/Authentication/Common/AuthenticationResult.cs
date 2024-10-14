using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common;

public record AuthenticationResult
   (
       User user,
       string Token
    );
