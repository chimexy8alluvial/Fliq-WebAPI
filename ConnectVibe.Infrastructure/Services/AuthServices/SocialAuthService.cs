using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using ConnectVibe.Infrastructure.Authentication;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;

namespace ConnectVibe.Infrastructure.Services.AuthServices
{
    public class SocialAuthService : ISocialAuthService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;

        public SocialAuthService(IOptions<GoogleAuthSettings> googleAuthSettings)
        {
            _googleAuthSettings = googleAuthSettings.Value;
        }

        public async Task<GooglePayloadResponse> ExchangeCodeForTokenAsync(string code)
        {
            GoogleAuthorizationCodeFlow authorizationCodeFlow = new GoogleAuthorizationCodeFlow(new GoogleAuthorizationCodeFlow.Initializer
            {
                ClientSecrets = new ClientSecrets()
                {
                    ClientId = _googleAuthSettings.ClientId,
                    ClientSecret = _googleAuthSettings.ClientSecret
                }
            });
            var tokenResponse = await authorizationCodeFlow.ExchangeCodeForTokenAsync(
       "user",
       code,
       _googleAuthSettings.RedirectUri,
       CancellationToken.None);

            return await ValidateGoogleToken(tokenResponse.IdToken);
        }

        public async Task<GooglePayloadResponse> ValidateGoogleToken(string token)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string>() { _googleAuthSettings.ClientId }
            };
            var payload = await GoogleJsonWebSignature.ValidateAsync(token, settings);

            GooglePayloadResponse googlePayloadResponse = new GooglePayloadResponse
            {
                Email = payload.Email,
                EmailVerified = payload.EmailVerified,
                Name = payload.Name,
                Picture = payload.Picture,
                FamilyName = payload.FamilyName,
                GivenName = payload.GivenName,
                Issuer = payload.Issuer
            };

            return googlePayloadResponse;
        }
    }
}