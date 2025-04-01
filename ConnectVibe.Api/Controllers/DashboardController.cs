using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.ActiveUserCount;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Application.DashBoard.Queries.InActiveUserCount;
using Fliq.Application.DashBoard.Queries.NewSignUpsCount;
using Fliq.Application.DashBoard.Queries.UsersCount;
using Fliq.Contracts.DashBoard;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin,SuperAdmin")]
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
            _logger.LogInfo("Received request for inactive users count.");

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

        [HttpGet("dashboard-tickets")]
        public async Task<IActionResult> GetEventsTicketsForDashboard([FromQuery] GetEventsTicketsListRequest request)
        {
            _logger.LogInfo($"Get Events Tickets Request Received: {request}");

            var query = new GetAllEventsTicketsQuery(request);

            var result = await _mediator.Send(query);
            _logger.LogInfo($"Query Executed. Result: {result}");

            return result.Match(
                events => Ok(_mapper.Map<List<GetEventsTicketsResponse>>(events)),
                errors => Problem(errors)
            );
        }
    }



}

