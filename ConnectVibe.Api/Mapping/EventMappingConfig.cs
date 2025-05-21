using Fliq.Application.Event.Commands.AddEventReview;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Application.Event.Commands.UpdateTicket;
using Fliq.Application.Event.Common;
using Fliq.Contracts.Event;
using Fliq.Contracts.Event.UpdateDtos;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class EventMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateEventRequest, CreateEventCommand>()
                   .Map(dest => dest.EventType, src => (EventType)src.EventType)
                   .Map(dest => dest.EventCategory, src => (EventCategory)src.EventCategory)
                   .Map(dest => dest.Tickets, src => src.Tickets)
                   .Ignore(dest => dest.MediaDocuments)
                .IgnoreNullValues(true);
            config.NewConfig<CreateEventCommand, Events>();
            config.NewConfig<CreateEventResult, CreateEventResponse>()
                .Map(dest => dest, src => src.Events);
            config.NewConfig<CreateEventResult, GetEventResponse>()
     .Map(dest => dest.Id, src => src.Events.Id) // Assuming Id exists in Events
     .Map(dest => dest.EventType, src => (int?)src.Events.EventType)
     .Map(dest => dest.EventCategory, src => (int?)src.Events.EventCategory)
     .Map(dest => dest.EventTitle, src => src.Events.EventTitle ?? string.Empty)
     .Map(dest => dest.EventDescription, src => src.Events.EventDescription)
     .Map(dest => dest.Location, src => src.Events.Location)
     .Map(dest => dest.Media, src => src.Events.Media)
     .Map(dest => dest.SponsoredEventDetail, src => src.Events.SponsoredEventDetail)
     .Map(dest => dest.EventCriteria, src => src.Events.EventCriteria)
     .Map(dest => dest.Tickets, src => src.Events.Tickets)
     .Map(dest => dest.EventPaymentDetail, src => src.Events.EventPaymentDetail)
     .Map(dest => dest.Capacity, src => src.Events.Capacity)
     .Map(dest => dest.StartDate, src => src.Events.StartDate)
     .Map(dest => dest.EndDate, src => src.Events.EndDate)
     .Map(dest => dest.MinAge, src => src.Events.MinAge)
     .Map(dest => dest.MaxAge, src => src.Events.MaxAge)
     .Map(dest => dest.SponsoredEvent, src => src.Events.SponsoredEvent)
     .Map(dest => dest.UserId, src => src.Events.UserId)
     .Map(dest => dest.InviteesException, src => src.Events.InviteesException);
            config.NewConfig<UpdateDiscountDto, DiscountDto>();
            config.NewConfig<UpdateEventDto, UpdateEventCommand>()
                 .Map(dest => dest.EventId, src => src.Id)
                     .Map(dest => dest.EventType, src => (EventType?)src.EventType)
                   .Map(dest => dest.EventCategory, src => (EventCategory?)src.EventCategory)
                   .Ignore(dest => dest.MediaDocuments);
                   
            config.NewConfig<UpdateTicketDto, UpdateTicketCommand>().IgnoreNullValues(true);

            config.NewConfig<UpdateDiscountDto, Discount>().IgnoreNullValues(true)
                .Map(dest => dest.Type, src => (DiscountType?)src.Type);

            config.NewConfig<AddTicketDto, AddTicketCommand>()
                .Map(dest => dest.TicketName, src => src.TicketName)
                .Map(dest => dest.TicketType, src => (TicketType)src.TicketType)
                .Map(dest => dest.TicketDescription, src => src.TicketDescription)
                .Map(dest => dest.EventDate, src => src.EventDate)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.MaximumLimit, src => src.MaximumLimit)
                .Map(dest => dest.SoldOut, src => src.SoldOut)
                .Map(dest => dest.Discounts, src => src.Discounts)
                .Map(dest => dest.EventId, src => src.EventId);

            // AddTicketCommand to Ticket
            config.NewConfig<AddTicketCommand, Ticket>()
                .Map(dest => dest.TicketName, src => src.TicketName)
                .Map(dest => dest.TicketType, src => src.TicketType)
                .Map(dest => dest.TicketDescription, src => src.TicketDescription)
                .Map(dest => dest.EventDate, src => src.EventDate)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.MaximumLimit, src => int.Parse(src.MaximumLimit))
                .Map(dest => dest.SoldOut, src => src.SoldOut)
                .Map(dest => dest.Discounts, src => src.Discounts)
                .Map(dest => dest.EventId, src => src.EventId)
                .Ignore(dest => dest.CurrencyId)
                .Ignore(dest => dest.Currency)
                .Ignore(dest => dest.Event)
                .Ignore(dest => dest.EventTickets);

            // CreateTicketResult to UpdateTicketResponse
            config.NewConfig<CreateTicketResult, UpdateTicketResponse>()
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.TicketName, src => src.TicketName)
                .Map(dest => dest.TicketType, src => (int)src.TicketType)
                .Map(dest => dest.TicketDescription, src => src.TicketDescription)
                .Map(dest => dest.EventDate, src => src.EventDate)
                .Map(dest => dest.Currency, src => src.Currency.CurrencyCode)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.MaximumLimit, src => src.MaximumLimit)
                .Map(dest => dest.SoldOut, src => src.SoldOut)
                .Map(dest => dest.Discounts, src => src.Discounts)
                .Map(dest => dest.EventId, src => src.EventId);

            config.NewConfig<UpdateTicketDto, UpdateTicketCommand>()
                .Map(dest => dest.EventId, src => src.EventId)
                .Map(dest => dest.Id, src => src.Id)
                .Map(dest => dest.TicketName, src => src.TicketName)
                .Map(dest => dest.TicketType, src => (int)src.TicketType)
                .Map(dest => dest.TicketDescription, src => src.TicketDescription)
                .Map(dest => dest.EventDate, src => src.EventDate)
                .Map(dest => dest.Amount, src => src.Amount)
                .Map(dest => dest.MaximumLimit, src => src.MaximumLimit)
                .Map(dest => dest.SoldOut, src => src.SoldOut)
                .Map(dest => dest.Discounts, src => src.Discounts);

            config.NewConfig<AddTicketDto, AddTicketCommand>();
            config.NewConfig<PurchaseTicketDto, AddEventTicketCommand>();
            config.NewConfig<AddEventReviewDto, AddEventReviewCommand>();
            config.NewConfig<AddReviewResult, GetEventResponse>();
            config.NewConfig<EventInviteeDto, EventInvitee>();
            config.NewConfig<EventCriteriaDto, EventCriteria>()
                .Map(dest => dest.Gender, src => src.Gender)
                .Map(dest => dest.EventType, src => src.EventType);
            config.NewConfig<TicketDto, Ticket>()
                .Map(dest => dest.TicketType, src => (TicketType)src.TicketType);
            config.NewConfig<DiscountDto, Discount>()
                .Map(dest => dest.Type, src => (DiscountType)src.Type);
            config.NewConfig<SponsoredEventDetailsDto, SponsoredEventDetail>()
                .Map(dest => dest.PreferedLevelOfInvolvement, src => (LevelOfInvolvement)src.PreferredLevelOfInvolvement)
                .Map(dest => dest.SponsoringPlan, src => (SponsoringPlan)src.SponsoringPlan)
                .Map(dest => dest.TargetAudienceType, src => (TargetAudienceType)src.TargetAudienceType);
            config.NewConfig<EventInviteeDto, EventInvitee>();
            config.NewConfig<EventPaymentDetailDto, EventPaymentDetail>();
        }
    }
}