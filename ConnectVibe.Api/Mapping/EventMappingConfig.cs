using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Application.Event.Commands.UpdateTicket;
using Fliq.Application.Event.Common;
using Fliq.Contracts.Event;
using Fliq.Contracts.Event.UpdateDtos;
using Fliq.Domain.Entities.Event;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class EventMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<CreateEventRequest, CreateEventCommand>();
            config.NewConfig<CreateEventCommand, Events>();
            config.NewConfig<CreateEventResult, CreateEventResponse>();
            config.NewConfig<CreateEventResult, GetEventResponse>();
            config.NewConfig<UpdateDiscountDto, DiscountDto>();
            config.NewConfig<UpdateEventDto, UpdateEventCommand>().IgnoreNullValues(true);
            config.NewConfig<UpdateTicketDto, UpdateTicketCommand>().IgnoreNullValues(true);
            config.NewConfig<AddTicketDto, AddTicketCommand>();
            config.NewConfig<PurchaseTicketDto, AddEventTicketCommand>();
        }
    }
}