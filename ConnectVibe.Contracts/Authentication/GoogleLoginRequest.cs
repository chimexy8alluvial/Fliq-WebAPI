namespace Fliq.Contracts.Authentication;
public record GoogleLoginRequest(string Code, string? DisplayName, int Language, string Theme);