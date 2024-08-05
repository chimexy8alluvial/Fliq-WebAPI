using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Common.Interfaces.Authentication
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(User user);
    }
}
