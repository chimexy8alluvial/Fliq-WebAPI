﻿namespace Fliq.Domain.Entities
{
    public class OTP : Record
    {
        public int UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
        public User User { get; set; }
    }
}