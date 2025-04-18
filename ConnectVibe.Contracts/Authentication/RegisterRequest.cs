﻿namespace Fliq.Contracts.Authentication;

public record RegisterRequest(
    string FirstName,
    string LastName,
    string? DisplayName,
    string Email,
    string Password,
    int Language,
    int Theme,
    string? PhoneNumber
    );