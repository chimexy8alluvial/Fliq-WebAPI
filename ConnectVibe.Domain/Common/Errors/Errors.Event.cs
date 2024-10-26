using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Event
        {
            public static Error EventNotFound => Error.Failure(
             code: "Event.EventNotFound",
             description: "Event with given id was not found.");
        }
    }
}