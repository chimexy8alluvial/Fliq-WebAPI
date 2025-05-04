using Fliq.Application.Common.Pagination;
using Fliq.Application.HelpAndSupport.Commands.AddMessage;
using Fliq.Application.HelpAndSupport.Commands.Create;
using Fliq.Application.HelpAndSupport.Queries.GetSupportTicket;
using Fliq.Application.HelpAndSupport.Queries.GetSupportTickets;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [ApiController]
    [Route("api/supporttickets")]
    public class SupportTicketController : ApiBaseController
    {
        private readonly IMediator _mediator;

        public SupportTicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> CreateSupportTicket([FromBody] CreateSupportTicketCommand command)
        {
            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return Ok(result.Value);
        }

        [HttpPost("messages")]
        public async Task<IActionResult> AddMessageToTicket([FromBody] AddMessageToTicketCommand command)
        {
            var senderid = GetAuthUserId();
            command.SenderId = senderid; // Ensure the ticket ID is set from the route
            var result = await _mediator.Send(command);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return NoContent();
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedSupportTickets(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] int? requestType = null,
            [FromQuery] int? requestStatus = null)
        {
            var query = new GetPaginatedSupportTicketsQuery
            {
                PaginationRequest = new PaginationRequest { PageNumber = pageNumber, PageSize = pageSize },
                RequestType = requestType,
                RequestStatus = requestStatus
            };

            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return Ok(result.Value);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSupportTicket(string id)
        {
            var query = new GetSupportTicketQuery { SupportTicketId = id };
            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return Ok(result.Value);
        }
    }
}