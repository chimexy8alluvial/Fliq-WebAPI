﻿using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record CreateEventRequest
    (
        int Id,
        EventType EventType,
        string eventTitle,
        string eventDescription,
        DateTime startDate,
        DateTime endDate,
        //TimeZoneInfo timeZone,
        LocationDto Location,
        int capacity,
        string optional,
        int UserId,
        List<EventDocumentDto> Docs,
        List<ProfilePhotoDto> Photos,
        string StartAge,
        string EndAge
    );

    //public enum EventType
    //{
    //    Physical,
    //    Live
    //}
}
