﻿using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.ActiveUserCount;
using Fliq.Application.DashBoard.Queries.EventsCount;
using Fliq.Application.DashBoard.Queries.FemaleUsersCount;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Application.DashBoard.Queries.InActiveUserCount;
using Fliq.Application.DashBoard.Queries.MaleUsersCount;
using Fliq.Application.DashBoard.Queries.NewSignUpsCount;
using Fliq.Application.DashBoard.Queries.OtherUsersCount;
using Fliq.Application.DashBoard.Queries.SponsoredEventsCount;
using Fliq.Application.DashBoard.Queries.UsersCount;
using Fliq.Contracts.DashBoard;
using Fliq.Contracts.Games;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Printing;

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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
             matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<EventCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<EventCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
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
              matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
              errors => Problem(errors)
          );
        }
        
        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsersForDashBoard([FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            _logger.LogInfo("Get All Users Request Received");

            var query = new GetAllUsersQuery(pageNumber, pageSize);
            var result = await _mediator.Send(query);

            _logger.LogInfo($"Get All Users Query Executed. Result: {result.Count} users found.");

            return Ok(result);
        }
    }
}
