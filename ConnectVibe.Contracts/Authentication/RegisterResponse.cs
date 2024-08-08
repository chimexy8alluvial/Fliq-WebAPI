﻿namespace ConnectVibe.Contracts.Authentication
{
    public record RegisterResponse(
           int Id,
    string FirstName,
    string LastName,
    string DisplayName,
    string Email,
    string Otp
    );
}
