using Newtonsoft.Json;

namespace Fliq.Application.Authentication.Common
{
    public class FacebookTokenValidationResponse
    {
        [JsonProperty("data")]
        public FacebookTokenValidationData Data { get; set; } = new();
    }

    public class FacebookTokenValidationData
    {
        [JsonProperty("app_id")]
        public string AppId { get; set; } = default!;

        [JsonProperty("type")]
        public string Type { get; set; } = default!;

        [JsonProperty("application")]
        public string Application { get; set; } = default!;

        [JsonProperty("data_access_expires_at")]
        public long DataAccessExpiresAt { get; set; } = default!;

        [JsonProperty("expires_at")]
        public long ExpiresAt { get; set; } = default!;

        [JsonProperty("is_valid")]
        public bool IsValid { get; set; }

        [JsonProperty("metadata")]
        public Metadata Metadata { get; set; } = new();

        [JsonProperty("scopes")]
        public string[] Scopes { get; set; } = default!;

        [JsonProperty("user_id")]
        public string UserId { get; set; } = default!;
    }

    public class Metadata
    {
        [JsonProperty("auth_type")]
        public string AuthType { get; set; } = default!;
    }
}