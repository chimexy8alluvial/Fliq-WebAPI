﻿using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Event.Commands.AddEventReview;
using Fliq.Application.Event.Commands.AddEventTicket;
using Fliq.Application.Event.Commands.ApproveEvent;
using Fliq.Application.Event.Commands.CancelEvent;
using Fliq.Application.Event.Commands.DeleteEvent;
using Fliq.Application.Event.Commands.EventCreation;
using Fliq.Application.Event.Commands.FlagEvent;
using Fliq.Application.Event.Commands.Tickets;
using Fliq.Application.Event.Commands.UpdateEvent;
using Fliq.Application.Event.Commands.UpdateTicket;
using Fliq.Application.Event.Common;
using Fliq.Application.Event.Queries.GetCurrency;
using Fliq.Application.Event.Queries.GetEvent;
using Fliq.Application.Event.Queries.GetTicket;
using Fliq.Contracts.Event;
using Fliq.Contracts.Event.ResponseDtos;
using Fliq.Contracts.Event.UpdateDtos;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
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
            command.MediaDocuments = request.MediaDocuments.Select(x => new EventMediaMapped
            {
                DocFile = x.DocFile,
                Title = x.Title
            }).ToList();

            command.UserId = GetAuthUserId();
            var EventCreatedResult = await _mediator.Send(command);
            _logger.LogInfo($"EventCreatedResult command Executed. Result: {EventCreatedResult}");

            return EventCreatedResult.Match(
                CreateEventResult => Ok(_mapper.Map<CreateEventResponse>(CreateEventResult)),
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

        [HttpPost("add-review")]
        public async Task<IActionResult> AddReview([FromForm] AddEventReviewDto request)
        {
            _logger.LogInfo($"Add Review Request Received: {request}");
            var command = _mapper.Map<AddEventReviewCommand>(request);
            var reviewResult = await _mediator.Send(command);
            _logger.LogInfo($" command Executed. Result: {reviewResult}");

            return reviewResult.Match(
                reviewResult => Ok(_mapper.Map<GetEventResponse>(reviewResult)),
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
            _logger.LogInfo($"Command Executed. Result: {ticketResult}");

            return ticketResult.Match(
                result => Ok(_mapper.Map<GetEventTicketResponse>(result)),
                errors => Problem(
                    detail: errors.First().Description,
                    title: errors.First().Code
                )
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
            );
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("flag-event/{eventId}")]
        public async Task<IActionResult> FlagEventById(int eventId)
        {
            _logger.LogInfo($"Flag event with ID: {eventId} received");

            var command = new FlagEventCommand(eventId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Flag event with ID: {eventId} executed. Result: {result} ");

            return result.Match(
              flagEventResult => Ok(new EventCommandResponse($"Event with ID: {eventId} was successfully flagged")),
              errors => Problem(errors)
          );

        }    
        
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("cancel-event/{eventId}")]
        public async Task<IActionResult> CancelEventById(int eventId)
        {
            _logger.LogInfo($"Cancel event with ID: {eventId} received");

            var command = new CancelEventCommand(eventId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Cancel event with ID: {eventId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok(new EventCommandResponse($"Event with ID: {eventId} was successfully cancelled")),
              errors => Problem(errors)
          );

        }
        
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("approve-event/{eventId}")]
        public async Task<IActionResult> ApproveEventById(int eventId)
        {
            _logger.LogInfo($"Approve event with ID: {eventId} received");

            var command = new ApproveEventCommand(eventId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Approve event with ID: {eventId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok(new EventCommandResponse($"Event with ID: {eventId} was successfully approved")),
              errors => Problem(errors)
          );

        } 
        
        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("delete-event/{eventId}")]
        public async Task<IActionResult> DeleteEventById(int eventId)
        {
            _logger.LogInfo($"Delete event with ID: {eventId} received");

            var command = new DeleteEventCommand(eventId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Delete event with ID: {eventId} executed. Result: {result} ");

            return result.Match(
              deleteEventResult => Ok(new EventCommandResponse($"Event with ID: {eventId} was successfully deleted")),
              errors => Problem(errors)
          );

        }
    }
}