﻿using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.DashBoard.Queries.ActiveUserCount;
using Fliq.Application.DashBoard.Queries.EventsCount;
using Fliq.Application.DashBoard.Queries.FemaleUsersCount;
using Fliq.Application.DashBoard.Queries.GetAllEvents;
using Fliq.Application.DashBoard.Queries.GetAllUser;
using Fliq.Application.DashBoard.Queries.InActiveUserCount;
using Fliq.Application.DashBoard.Queries.MaleUsersCount;
using Fliq.Application.DashBoard.Queries.NewSignUpsCount;
using Fliq.Application.DashBoard.Queries.OtherUsersCount;
using Fliq.Application.DashBoard.Queries.SponsoredEventsCount;
using Fliq.Application.DashBoard.Queries.UsersCount;
using Fliq.Contracts.DashBoard;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text;

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

        [HttpGet("export-events-csv")]
        public async Task<IActionResult> ExportEventsToCsv([FromQuery] GetEventsListRequest request)
        {           
                _logger.LogInfo("Export events to CSV request received");
               
                var query = _mapper.Map<GetAllEventsQuery>(request);
                var result = await _mediator.Send(query);

                 return await result.MatchAsync<IActionResult>(
                      async eventsResult =>
                      {
                          var csvContent = await GenerateCsvContent(eventsResult);
                          var fileName = $"Events_Export_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
                          _logger.LogInfo($"Successfully generated CSV with {eventsResult.Count} events");
                          return File(
                              Encoding.UTF8.GetBytes(csvContent),
                              "text/csv",
                              fileName
                          );
                      },

                      errors => Task.FromResult(Problem(errors))
                 );
            
        }

        // Helper method to generate CSV content
        private async Task<string> GenerateCsvContent(List<GetEventsResult> events)
        {
            await Task.CompletedTask; 

            var sb = new StringBuilder();

            // Add CSV header
            sb.AppendLine("EventTitle,CreatedBy,Status,Attendees,EventCategory,CreatedOn");

            // Add data rows
            foreach (var eventItem in events)
            {
                // Escape values that might contain commas or quotes
                string eventTitle = $"\"{eventItem.EventTitle.Replace("\"", "\"\"")}\"";
                string createdBy = $"\"{eventItem.CreatedBy.Replace("\"", "\"\"")}\"";
                string status = $"\"{eventItem.Status.Replace("\"", "\"\"")}\"";
                string eventCategory = $"\"{eventItem.EventCategory.Replace("\"", "\"\"")}\"";

                sb.AppendLine(
                    $"{eventTitle}," +
                    $"{createdBy}," +
                    $"{status}," +
                    $"{eventItem.Attendees}," +
                    $"{eventCategory}," +
                    $"{eventItem.CreatedOn:yyyy-MM-dd HH:mm:ss}"
                );
            }

            return sb.ToString();
        }


        [HttpGet("get-all-cancelled-events")]
         public async Task<IActionResult> GetAllCancelledEventsForDashBoard([FromQuery] GetEventsListRequest request)
         {
             _logger.LogInfo("Get all events request received");

             var query = _mapper.Map<GetAllCancelledEventsQuery>(request);
             var result = await _mediator.Send(query);

             _logger.LogInfo($"Get all events query executed. Result: {result} ");

             return result.Match(
               getAllCancelledEventsResult => Ok(_mapper.Map<List<GetEventsResponse>>(result.Value)),
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

    }
}
