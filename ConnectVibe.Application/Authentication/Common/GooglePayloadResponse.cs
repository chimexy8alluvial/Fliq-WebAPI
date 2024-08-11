using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Authentication.Common
{
    public class GooglePayloadResponse
    {
        public string Issuer { get; set; }           // The token's issuer
        public string Audience { get; set; }         // Your application's client ID
        public string Subject { get; set; }          // The user's Google ID
        public string Email { get; set; }            // The user's email address
        public bool EmailVerified { get; set; }      // Whether the user's email is verified
        public string Name { get; set; }             // The user's full name
        public string Picture { get; set; }          // The URL of the user's profile picture
        public string Locale { get; set; }           // The user's locale
        public string GivenName { get; set; }        // The user's first name
        public string FamilyName { get; set; }       // The user's last name
        public long Expiry { get; set; }             // Expiration time of the token
        public long IssuedAt { get; set; }           // Issue time of the token
        public string JwtId { get; set; }
    }
}