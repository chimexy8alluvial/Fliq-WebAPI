using ConnectVibe.Contracts.Profile;

namespace Fliq.Contracts.Event
{
    public record TicketTypeDto
    (
        string TicketName,
        string TicketDescription,
        string OpensOn,
        string ClosesOn,
        string TimeZone,
        LocationDto Location,
        string TicketTypes,
        string Currency,
        double Amount
    );
}
