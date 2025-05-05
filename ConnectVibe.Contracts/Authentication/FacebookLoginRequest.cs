namespace Fliq.Contracts.Authentication
{
    public record FacebookLoginRequest(string Code, string? DisplayName, int Language, string Theme);
}