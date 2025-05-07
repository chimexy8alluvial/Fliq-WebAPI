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
                   .Ignore(dest => dest.MediaDocuments)
                .IgnoreNullValues(true);
            config.NewConfig<CreateEventCommand, Events>();
            config.NewConfig<CreateEventResult, CreateEventResponse>()
                .Map(dest => dest, src => src.Events);
            config.NewConfig<CreateEventResult, GetEventResponse>();
            config.NewConfig<UpdateDiscountDto, DiscountDto>();
            config.NewConfig<UpdateEventDto, UpdateEventCommand>()
                 .Map(dest => dest.EventId, src => src.Id)
                     .Map(dest => dest.EventType, src => (EventType?)src.EventType)
                   .Map(dest => dest.EventCategory, src => (EventCategory?)src.EventCategory)
                   .Ignore(dest => dest.MediaDocuments);
                   
            config.NewConfig<UpdateTicketDto, UpdateTicketCommand>().IgnoreNullValues(true);

            config.NewConfig<UpdateDiscountDto, Discount>().IgnoreNullValues(true)
                .Map(dest => dest.Type, src => (DiscountType?)src.Type);

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