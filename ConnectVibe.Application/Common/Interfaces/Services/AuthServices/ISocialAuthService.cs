using ConnectVibe.Application.Authentication.Common;

namespace ConnectVibe.Application.Common.Interfaces.Services.AuthServices
{
    public interface ISocialAuthService
    {
        Task<GooglePayloadResponse> ExchangeCodeForTokenAsync(string code);

        Task<FacebookUserInfoResponse?> GetFacebookUserInformation(string accessToken);
    }
}