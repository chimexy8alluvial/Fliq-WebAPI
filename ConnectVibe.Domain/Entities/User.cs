﻿using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Entities.Notifications;
using Fliq.Domain.Entities.Profile;
using Fliq.Domain.Entities.Settings;

namespace Fliq.Domain.Entities
{
    public class User : Record
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string PasswordSalt { get; set; } = string.Empty;
        public bool IsEmailValidated { get; set; }
        public bool IsDocumentVerified { get; set; }
        public bool IsActive { get; set; }
        public DateTime LastActiveAt { get; set; } = DateTime.UtcNow;
        public int RoleId { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ContactInformation { get; set; } = string.Empty;
        public UserProfile? UserProfile { get; set; }
        public Setting? Settings { get; set; }
        public Role? Role { get; set; }
        public string? BusinessName { get; set; } = string.Empty;
        public string? BusinessType { get; set; } = string.Empty;
        public string? BusinessAddress { get; set; } = string.Empty;
        public string? CompanyBio { get; set; } = string.Empty;
        public List<Payment>? Payments { get; set; }
        public List<Subscription>? Subscriptions { get; set; }
        public List<MatchRequest>? MatchRequests { get; set; }
        public ICollection<UserDeviceToken>? DeviceTokens { get; set; }
    }
}