﻿using ConnectVibe.Domain.Entities.Profile;

namespace Fliq.Domain.Entities.Event
{
    public record EventDetailsResults
    (
        int Id,
        string Email,
        string eventTitle,
        string eventDescription,
        DateTime startDate,
        DateTime endDate,
        TimeZoneInfo timeZone,
        Location Location,
        int capacity,
        string optional
    );
}
