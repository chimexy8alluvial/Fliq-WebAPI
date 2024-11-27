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
            config.NewConfig<CreateEventRequest, CreateEventCommand>().IgnoreNullValues(true);
            config.NewConfig<CreateEventCommand, Events>();
            config.NewConfig<CreateEventResult, CreateEventResponse>();
            config.NewConfig<CreateEventResult, GetEventResponse>();
            config.NewConfig<UpdateDiscountDto, DiscountDto>();
            config.NewConfig<UpdateEventDto, UpdateEventCommand>().IgnoreNullValues(true);
            config.NewConfig<UpdateTicketDto, UpdateTicketCommand>().IgnoreNullValues(true);

            config.NewConfig<UpdateDiscountDto, Discount>().IgnoreNullValues(true)
                .Map(dest => dest.Type, src => (DiscountType)src.Type);

            config.NewConfig<AddTicketDto, AddTicketCommand>();
            config.NewConfig<PurchaseTicketDto, AddEventTicketCommand>();
            config.NewConfig<AddEventReviewDto, AddEventReviewCommand>();
            config.NewConfig<AddReviewResult, GetEventResponse>();
            config.NewConfig<EventInviteeDto, EventInvitee>();
        }
    }
}