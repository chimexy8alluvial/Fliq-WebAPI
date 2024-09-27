using Fliq.Application.Authentication.Common;

namespace Fliq.Application.Common.Interfaces.Services.AuthServices
{
    public interface ISocialAuthService
    {
        Task<GooglePayloadResponse> ExchangeCodeForTokenAsync(string code);

        Task<FacebookUserInfoResponse?> GetFacebookUserInformation(string accessToken);
    }
}