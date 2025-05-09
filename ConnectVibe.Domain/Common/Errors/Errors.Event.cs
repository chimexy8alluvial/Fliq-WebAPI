﻿using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Event
        {
            public static Error EventNotFound => Error.Failure(
             code: "Event.EventNotFound",
             description: "Event with given id was not found.");

            public static Error InsufficientCapacity => Error.Failure(
                code: "Event.InsufficientCapacity",
                description: "The event does not have enough capacity for the requested number of tickets.");

            public static Error NoAvailableSeats => Error.Failure(
                code: "Event.NoAvailableSeats",
                description: "No available seats left for the event.");
            
            public static Error EventFlaggedAlready => Error.Failure(
                code: "Event.EventFlaggedAlready",
                description: "Event with given id has been flagged already.");


            public static Error EventCancelledAlready => Error.Failure(
                code: "Event.EventCancelledAlready",
                description: "Event with given id has been cancelled already.");
            public static Error EventEndedAlready => Error.Failure(
                code: "Event.EventEndedAlready",
                description: "Event with given id has ended.");
            
            public static Error EventApprovedAlready => Error.Failure(
                code: "Event.EventApprovedAlready",
                description: "Event with given id has been approved already.");
            
            public static Error EventDeletedAlready => Error.Failure(
                code: "Event.EventDeletedAlready",
                description: "Event with given id has been deleted already.");
        }
    }
}