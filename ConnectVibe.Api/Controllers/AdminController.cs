﻿using Fliq.Application.Authentication.Commands.CreateAdmin;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Users.Commands;
using Fliq.Application.Users.Queries;
using Fliq.Contracts.Authentication;
using Fliq.Contracts.Common;
using Fliq.Contracts.Users;
using Fliq.Contracts.Users.UserFeatureActivities;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public AdminController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = "SuperAdmin")]
        [HttpPost("create-admin")]
        public async Task<IActionResult> CreateAdmin([FromBody] RegisterRequest request)
        {
            _logger.LogInfo($"Admin Creation Request Received: {request}");
            var command = _mapper.Map<CreateAdminCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Create Admin Command Executed. Result: {authResult}");
            return authResult.Match(
                authResult => Ok(_mapper.Map<RegisterResponse>(authResult)),
                errors => Problem(errors)
            );
        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("deactivate-user/{UserId}")]
        public async Task<IActionResult> DeactivateUser(int UserId)
        {
            _logger.LogInfo($"User Deactivation Request Received for User with Id: {UserId}");
            var command = new DeactivateUserCommand(UserId);
            var result = await _mediator.Send(command);
            _logger.LogInfo($"User Deactivation Command Executed. Result:  {result}");

            return result.Match(
                result => Ok(new BasicActionResponse($"User with ID {UserId} deactivated successfully")),
                errors => Problem(errors)
            );
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("get-recent-user-activity")]
        public async Task<IActionResult> GetRecentUserFeatureActivity(GetRecentUserFeatureActivityRequest request)
        {
            _logger.LogInfo($"Executing Request for Recent FeatureActivity for User with Id: {request.UserId}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var query = new GetRecentUserFeatureActivitiesQuery(userId, request.UserId, request.Limit);
            var result = await _mediator.Send(query);
            _logger.LogInfo($"User Recent Feature Activities Query Executed. Result:  {result}");

            return result.Match(
                result => Ok(_mapper.Map<List<GetRecentUserFeatureActivityResponse>>(result)),
                errors => Problem(errors)
            );
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpGet("export/users")]
        public async Task<IActionResult> ExportUsers([FromQuery] int roleId)
        {
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var query = new ExportUsersToCsvQuery(userId, roleId);

            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return result.FirstError switch
                {
                    _ => StatusCode(500, "An unexpected error occurred.")
                };
            }

            return File(new MemoryStream(result.Value), "text/csv", "users_export.csv");
        }
    }
}
