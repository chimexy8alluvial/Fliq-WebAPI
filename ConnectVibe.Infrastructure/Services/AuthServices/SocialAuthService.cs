using ConnectVibe.Application.Authentication.Common;
using ConnectVibe.Application.Common.Interfaces.Services.AuthServices;
using ConnectVibe.Infrastructure.Authentication;
using Google.Apis.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Auth.OAuth2.Flows;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace ConnectVibe.Infrastructure.Services.AuthServices
{
    public class SocialAuthService : ISocialAuthService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly FacebookAuthSettings _facebookAuthSettings;
        private readonly IHttpClientFactory _httpClientFactory;

        public SocialAuthService(IOptions<GoogleAuthSettings> googleAuthSettings, IOptions<FacebookAuthSettings> facebookAuthSettings, IHttpClientFactory httpClientFactory)
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _facebookAuthSettings = facebookAuthSettings.Value;
            _httpClientFactory = httpClientFactory;
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

        public async Task<FacebookTokenValidationResponse?> ValidateFacebookToken(string accessToken)
        {
            string TokenValidationUrl = _facebookAuthSettings.TokenValidationUrl;
            var url = string.Format(TokenValidationUrl, accessToken, _facebookAuthSettings.AppId, _facebookAuthSettings.AppSecret);

            var client = _httpClientFactory.CreateClient("Facebook");
            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();

                var tokenValidationResponse = JsonConvert.DeserializeObject<FacebookTokenValidationResponse>(responseAsString);
                return tokenValidationResponse;
            }

            return null;
        }

        public async Task<FacebookUserInfoResponse?> GetFacebookUserInformation(string accessToken)
        {
            string userInfoUrl = _facebookAuthSettings.UserInfoUrl;
            string url = string.Format(userInfoUrl, accessToken);
            var client = _httpClientFactory.CreateClient("Facebook");

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseAsString = await response.Content.ReadAsStringAsync();
                var userInfoResponse = JsonConvert.DeserializeObject<FacebookUserInfoResponse>(responseAsString);
                return userInfoResponse;
            }
            return null;
        }

        private async Task<GooglePayloadResponse> ValidateGoogleToken(string token)
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