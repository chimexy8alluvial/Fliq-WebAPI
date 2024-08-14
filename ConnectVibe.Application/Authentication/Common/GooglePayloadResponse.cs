namespace ConnectVibe.Application.Authentication.Common
{
    public class GooglePayloadResponse
    {
        public string Issuer { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Subject { get; set; } = default!;
        public string Email { get; set; } = default!;
        public bool EmailVerified { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Picture { get; set; } = default!;
        public string Locale { get; set; } = default!;
        public string GivenName { get; set; } = default!;
        public string FamilyName { get; set; } = default!;
        public long Expiry { get; set; } = default!;
        public long IssuedAt { get; set; } = default!;
        public string JwtId { get; set; } = default!;
    }
}