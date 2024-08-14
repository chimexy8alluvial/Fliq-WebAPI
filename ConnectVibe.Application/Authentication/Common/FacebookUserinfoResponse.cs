using Newtonsoft.Json;

namespace ConnectVibe.Application.Authentication.Common
{
    public class FacebookUserInfoResponse
    {
        [JsonProperty("id")]
        public string Id { get; set; } = default!;

        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("first_name")]
        public string FirstName { get; set; } = default!;

        [JsonProperty("last_name")]
        public string LastName { get; set; } = default!;

        [JsonProperty("email")]
        public string Email { get; set; } = default!;

        [JsonProperty("picture")]
        public Picture Picture { get; set; } = default!;
    }

    public class Picture
    {
        [JsonProperty("data")]
        public Data Data { get; set; } = default!;
    }

    public class Data
    {
        [JsonProperty("height")]
        public long Height { get; set; }

        [JsonProperty("is_silhouette")]
        public bool IsSilhouette { get; set; }

        [JsonProperty("url")]
        public Uri Url { get; set; } = default!;

        [JsonProperty("width")]
        public long Width { get; set; }
    }
}