using Fliq.Domain.Entities.Event;

namespace Fliq.Application.Event.Common
{
    public record CreateTicketResult
    {
        public int Id { get; init; }
        public string TicketName { get; init; } = default!;
        public TicketType TicketType { get; init; }
        public string TicketDescription { get; init; } = default!;
        public DateTime EventDate { get; init; }
        public Currency Currency { get; init; } = default!;
        public decimal Amount { get; init; }
        public string MaximumLimit { get; init; } = default!;
        public bool SoldOut { get; init; }
        public List<Discount>? Discounts { get; init; }
        public int EventId { get; init; }

        public CreateTicketResult(Ticket ticket)
        {
            Id = ticket.Id;
            TicketName = ticket.TicketName ?? string.Empty;
            TicketType = ticket.TicketType;
            TicketDescription = ticket.TicketDescription;
            EventDate = ticket.EventDate;
            Currency = ticket.Currency;
            Amount = ticket.Amount;
            MaximumLimit = ticket.MaximumLimit.ToString();
            SoldOut = ticket.SoldOut;
            Discounts = ticket.Discounts;
            EventId = ticket.EventId;
        }
    }
}