﻿using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Application.Event.Commands.UpdateTicket;
using Fliq.Application.Event.Queries.GetCurrency;
using Fliq.Application.Event.Queries.GetEvent;
using Fliq.Application.Event.Queries.GetTicket;
using Fliq.Contracts.Event;
using Fliq.Contracts.Event.ResponseDtos;
using Fliq.Contracts.Event.UpdateDtos;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/event")]
    public class EventController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IOtpRepository _otpRepository;
        private readonly ILoggerManager _logger;

        public EventController(ISender mediator, IMapper mapper, IOtpRepository otpRepository, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateEvent([FromForm] CreateEventRequest request)
        {
            _logger.LogInfo($"Create Request Received: {request}");
            var command = _mapper.Map<CreateEventCommand>(request);
            command.UserId = GetAuthUserId();
            var EventCreatedResult = await _mediator.Send(command);
            _logger.LogInfo($"EventCreatedResult command Executed. Result: {EventCreatedResult}");

            return EventCreatedResult.Match(
                CreateEventResult => Ok(_mapper.Map<CreateEventResponse>(EventCreatedResult)),
                errors => Problem(errors)
            );
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update([FromForm] UpdateEventDto request)
        {
            _logger.LogInfo($"Update Request  Received: {request}");
            var command = _mapper.Map<UpdateEventCommand>(request);
            var EventCreatedResult = await _mediator.Send(command);
            _logger.LogInfo($"Update command Executed. Result: {EventCreatedResult}");

            return EventCreatedResult.Match(
                CreateEventResult => Ok(_mapper.Map<GetEventResponse>(EventCreatedResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("create-ticket")]
        public async Task<IActionResult> CreateTicket([FromForm] AddTicketDto request)
        {
            _logger.LogInfo($"Create Request Received: {request}");
            var command = _mapper.Map<AddTicketCommand>(request);
            var ticketResult = await _mediator.Send(command);
            _logger.LogInfo($" command Executed. Result: {ticketResult}");

            return ticketResult.Match(
                ticketResult => Ok(_mapper.Map<UpdateTicketResponse>(ticketResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("purchase-ticket")]
        public async Task<IActionResult> PurchaseTicket([FromForm] PurchaseTicketDto request)
        {
            _logger.LogInfo($"Purchase Request Received: {request}");
            var command = _mapper.Map<AddEventTicketCommand>(request);
            command.UserId = GetAuthUserId();
            var ticketResult = await _mediator.Send(command);
            _logger.LogInfo($" command Executed. Result: {ticketResult}");

            return ticketResult.Match(
                ticketResult => Ok(_mapper.Map<GetEventTicketResponse>(ticketResult)),
                errors => Problem(errors)
            );
        }

        [HttpPut("update-ticket")]
        public async Task<IActionResult> UpdateTicket([FromForm] UpdateTicketDto request)
        {
            _logger.LogInfo($"Update Request  Received: {request}");
            var command = _mapper.Map<UpdateTicketCommand>(request);
            var ticketResult = await _mediator.Send(command);
            _logger.LogInfo($"Update command Executed. Result: {ticketResult}");

            return ticketResult.Match(
                ticketResult => Ok(_mapper.Map<UpdateTicketResponse>(ticketResult)),
                errors => Problem(errors)
            );
        }

        [HttpGet("{eventId}")]
        public async Task<IActionResult> GetEvent(int eventId)
        {
            _logger.LogInfo($"Get Event Request Received for Event ID: {eventId}");
            var query = new GetEventQuery(eventId);
            var eventResult = await _mediator.Send(query);
            _logger.LogInfo($"Get Event query executed. Result: {eventResult}");

            return eventResult.Match(
                ev => Ok(_mapper.Map<GetEventResponse>(ev)),
                errors => Problem(errors)
            ); 4
        }

        [HttpGet("ticket/{ticketId}")]
        public async Task<IActionResult> GetTicket(int ticketId)
        {
            _logger.LogInfo($"Get Ticket Request Received for Ticket ID: {ticketId}");
            var query = new GetTicketQuery(ticketId);
            var ticketResult = await _mediator.Send(query);
            _logger.LogInfo($"Get Ticket query executed. Result: {ticketResult}");

            return ticketResult.Match(
                ticket => Ok(_mapper.Map<GetTicketResponse>(ticket)),
                errors => Problem(errors)
            );
        }

        [HttpGet("currencies")]
        public async Task<IActionResult> GetCurrencies()
        {
            _logger.LogInfo($"Get Currency Request Received");
            var query = new GetCurrenciesQuery();
            var currenciesResult = await _mediator.Send(query);
            _logger.LogInfo($"Get Ticket query executed. Result: {currenciesResult}");

            return currenciesResult.Match(
                ticket => Ok(_mapper.Map<List<GetCurrencyResponse>>(ticket)),
                errors => Problem(errors)
            );
        }
    }
}