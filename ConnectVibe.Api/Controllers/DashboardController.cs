using ErrorOr;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries;
using Fliq.Application.DashBoard.Queries.ActiveUserCount;
using Fliq.Application.DashBoard.Queries.EventsCount;
using Fliq.Application.DashBoard.Queries.EventsWithPendingApproval;
using Fliq.Application.DashBoard.Queries.FemaleUsersCount;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Application.DashBoard.Queries.InActiveUserCount;
using Fliq.Application.DashBoard.Queries.MaleUsersCount;
using Fliq.Application.DashBoard.Queries.NewSignUpsCount;
using Fliq.Application.DashBoard.Queries.OtherTicketCount;
using Fliq.Application.DashBoard.Queries.RegularTicketCount;
using Fliq.Application.DashBoard.Queries.TotalTicketCount;
using Fliq.Application.DashBoard.Queries.OtherUsersCount;
using Fliq.Application.DashBoard.Queries.SponsoredEventsCount;
using Fliq.Application.DashBoard.Queries.UsersCount;
using Fliq.Application.DashBoard.Queries.VipTicketCount;
using Fliq.Application.DashBoard.Queries.VVipTicketCount;
using Fliq.Application.Event.Commands.RefundTicket;
using Fliq.Application.Event.Commands.StopTicketSales;
using Fliq.Contracts.DashBoard;
using Fliq.Domain.Entities.Event;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Fliq.Application.DashBoard.Queries.DailyTicketCount;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin,SuperAdmin")]
    public class DashboardController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public DashboardController(ISender mediator, ILoggerManager logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("active-users-count")]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            _logger.LogInfo("Received request for active users count.");

            var query = new GetActiveUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              matchedProfileResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("inactive-users-count")]
        public async Task<IActionResult> GetInactiveUsersCount()
        {
            _logger.LogInfo("Received request for inactive users count.");

            var query = new GetInActiveUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              matchedProfileResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("users-count")]
        public async Task<IActionResult> GetUsersCount()
        {
            _logger.LogInfo("Received request for  users count.");

            var query = new GetAllUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              matchedProfileResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("new-signups-count")]
        public async Task<IActionResult> GetNewSignupsCount([FromQuery] int days = 7)
        {
            _logger.LogInfo($"Received request for new signups count in the last {days} days.");

            var query = new GetNewSignUpsCountQuery(days);
            var result = await _mediator.Send(query);

            return result.Match(
             matchedProfileResult => Ok(_mapper.Map<CountResponse>(result.Value)),
             errors => Problem(errors)
         );
        }

        [HttpGet("male-users-count")]
        public async Task<IActionResult> GetMaleUsersCount()
        {
            _logger.LogInfo("Received request for  male-users count.");

            var query = new GetAllMaleUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              maleUsersCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("female-users-count")]
        public async Task<IActionResult> GetFemaleUsersCount()
        {
            _logger.LogInfo("Received request for  female-users count.");

            var query = new GetAllFemaleUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
             femaleUsersCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("other-users-count")]
        public async Task<IActionResult> GetOtherUsersCount()
        {
            _logger.LogInfo("Received request for  other-users count.");

            var query = new GetAllOtherUsersCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
             otherUsersCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsersForDashBoard([FromQuery] GetUsersListRequest request)
        {
            _logger.LogInfo("Get All Users Request Received");

            var query = _mapper.Map<GetAllUsersQuery>(request);
            var result = await _mediator.Send(query);

            _logger.LogInfo($"Get All Users Query Executed. Result: {result} ");

            return result.Match(
              getAllUsersResult => Ok(_mapper.Map<List<GetUsersResponse>>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("event-count")]
        public async Task<IActionResult> GetEventsCount()
        {
            _logger.LogInfo("Received request for events count.");

            var query = new GetAllEventsCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              eventCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
            );
        }


        [HttpGet("events-with-pending-approval-count")]
        public async Task<IActionResult> GetEventsWithPendingApprovalCount()
        {
            _logger.LogInfo("Received request for events with pending approval count.");

            var query = new GetAllEventsWithPendingApprovalCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
              eventCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
            );
        }

        [HttpGet("sponsored-event-count")]
        public async Task<IActionResult> GetSponsoredEventsCount()
        {
            _logger.LogInfo("Received request for Sponsored-events count.");

            var query = new GetAllSponsoredEventsCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
             sponsoredEventsCountResult => Ok(_mapper.Map<CountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }


        [HttpGet("get-all-events")]
        public async Task<IActionResult> GetAllEventsForDashBoard([FromQuery] GetEventsListRequest request)
        {
            _logger.LogInfo("Get all events request received");

            var query = _mapper.Map<GetAllEventsQuery>(request);
            var result = await _mediator.Send(query);

            _logger.LogInfo($"Get all events query executed. Result: {result} ");

            return result.Match(
              getAllEventsResult => Ok(_mapper.Map<List<GetEventsResponse>>(result.Value)),
              errors => Problem(errors)
          );
        }


        [HttpGet("get-all-flaggged-events")]
        public async Task<IActionResult> GetAllFlaggedEventsForDashBoard([FromQuery] GetEventsListRequest request)
        {
            _logger.LogInfo("Get all events request received");

            var query = _mapper.Map<GetAllFlaggedEventsQuery>(request);
            var result = await _mediator.Send(query);

            _logger.LogInfo($"Get all events query executed. Result: {result} ");

            return result.Match(
              getAllFlaggedEventsResult => Ok(_mapper.Map<List<GetEventsResponse>>(result.Value)),
              errors => Problem(errors)
          );
        }

        [HttpGet("dashboard-tickets")]
        public async Task<IActionResult> GetEventsTicketsForDashboard([FromQuery] GetEventsTicketsListRequest request)
        {
            _logger.LogInfo($"Get Events Tickets Request Received: {request}");

            var query = _mapper .Map<GetAllEventsTicketsQuery>(request);

            var result = await _mediator.Send(query);
            _logger.LogInfo($"Query Executed. Result: {result}");

            return result.Match(
                events => Ok(_mapper.Map<List<GetEventsTicketsResponse>>(events)),
                errors => Problem(errors)
            );
        }

        [HttpGet("regular-ticket-count")]
        public async Task<IActionResult> GetRegularTicketCount([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for regular ticket count for EventId: {eventId}");

            var query = new GetEventRegularTicketCountQuery(eventId);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("vip-ticket-count")]
        public async Task<IActionResult> GetVipTicketCount([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for VIP ticket count for EventId: {eventId}");

            var query = new GetEventVipTicketCountQuery(eventId);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("vvip-ticket-count")]
        public async Task<IActionResult> GetVVipTicketCount([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for VVIP ticket count for EventId: {eventId}");

            var query = new GetEventVVipTicketCountQuery(eventId);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("other-ticket-count")]
        public async Task<IActionResult> GetOtherTicketCount([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for Other ticket count for EventId: {eventId}");

            var query = new GetEventOtherTicketCountQuery(eventId);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("total-ticket-count")]
        public async Task<IActionResult> GetTotalTicketCount([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for total ticket count for EventId: {eventId}");

            var query = new GetEventTotalTicketCountQuery(eventId);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("monday-ticket-count")]
        public async Task<IActionResult> GetMondayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Monday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");

            var query = new GetMondayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);

            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("tuesday-ticket-count")]
        public async Task<IActionResult> GetTuesdayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Tuesday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetTuesdayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("wednesday-ticket-count")]
        public async Task<IActionResult> GetWednesdayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Wednesday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetWednesdayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("thursday-ticket-count")]
        public async Task<IActionResult> GetThursdayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Thursday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetThursdayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("friday-ticket-count")]
        public async Task<IActionResult> GetFridayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Friday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetFridayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("saturday-ticket-count")]
        public async Task<IActionResult> GetSaturdayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Saturday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetSaturdayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("sunday-ticket-count")]
        public async Task<IActionResult> GetSundayTicketCount([FromQuery] int eventId, [FromQuery] TicketType? ticketType = null)
        {
            _logger.LogInfo($"Received request for Sunday ticket count for EventId: {eventId}, TicketType: {ticketType?.ToString() ?? "All"}");
            var query = new GetSundayEventTicketCountQuery(eventId, ticketType);
            var result = await _mediator.Send(query);
            return result.Match(
                matchedResult => Ok(_mapper.Map<CountResponse>(result.Value)),
                errors => Problem(errors)
            );
        }

        [HttpGet("gross-revenue")]
        public async Task<IActionResult> GetEventTicketGrossRevenue([FromQuery] int eventId)
        {
            _logger.LogInfo($"Received request for gross revenue for EventId: {eventId}");
            var query = new GetEventTicketGrossRevenueQuery(eventId);
            var result = await _mediator.Send(query);
            return result.Match(
                revenue => Ok(new { Revenue = revenue }), // Simple JSON response
                errors => Problem(errors)
            );
        }
           

        [HttpGet("{eventId}/net-revenue")]
        public async Task<IActionResult> GetNetRevenue(int eventId)
        {
            var query = new GetEventTicketNetRevenueQuery(eventId);
            ErrorOr<decimal> result = await _mediator.Send(query);

            return result.Match(
                revenue => Ok(revenue),
                errors => Problem( errors)
            );
        }

        [HttpPost("refund")]
        public async Task<IActionResult> RefundTicket([FromBody] RefundTicketCommand command)
        {
            ErrorOr<RefundTicketResult> result = await _mediator.Send(command);

            return result.Match(
                refundResult => Ok(new { RefundedTickets = refundResult.RefundedTickets.Count }),
                errors => Problem(detail: errors.First().Description, statusCode: 400)
            );
        }

        [HttpPost("stop-sales")]
        public async Task<IActionResult> StopTicketSales([FromBody] StopTicketSalesCommand command)
        {
            ErrorOr<StopTicketSalesResult> result = await _mediator.Send(command);

            return result.Match(
                stopResult => Ok(new { UpdatedTicketCount = stopResult.UpdatedTicketCount }),
                errors => Problem(detail: errors.First().Description, statusCode: 400)
            );
        }
    }
}





