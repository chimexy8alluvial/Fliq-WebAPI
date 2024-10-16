using Fliq.Application.Common.Interfaces.Services.LocationServices;
using Fliq.Application.Common.Models;
using Fliq.Infrastructure.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Fliq.Infrastructure.Services.LocationServices
{
    public class LocationService : ILocationService
    {
        private readonly GoogleAuthSettings _googleAuthSettings;
        private readonly HttpClient _httpClient;

        public LocationService(IOptions<GoogleAuthSettings> googleAuthSettings, HttpClient httpClient)
        {
            _googleAuthSettings = googleAuthSettings.Value;
            _httpClient = httpClient;
        }

        public async Task<LocationQueryResponse?> GetAddressFromCoordinatesAsync(double latitude, double longitude)
        {
            var requestUrl = $"{_googleAuthSettings.LocationApiEndpoint}?latlng={latitude},{longitude}&key={_googleAuthSettings.LocationApiKey}";
            var response = await _httpClient.GetAsync(requestUrl);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var geocodingResponse = JsonSerializer.Deserialize<LocationQueryResponse>(jsonResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower
                });

                return geocodingResponse;
            }
            else
            {
                return null;
            }
        }
    }
}