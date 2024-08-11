using ConnectVibe.Application.Authentication.Common;

namespace ConnectVibe.Application.Common.Interfaces.Services.AuthServices
{
    public interface ISocialAuthService
    {
        Task<GooglePayloadResponse> ValidateGoogleToken(string token);

        Task<GooglePayloadResponse> ExchangeCodeForTokenAsync(string code);
    }
}