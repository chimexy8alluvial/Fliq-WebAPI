using ErrorOr;
using Fliq.Application.AuditTrailCommand;
using Fliq.Application.Authentication.Commands.CreateAdmin;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Command.DeleteUser;
using Fliq.Application.DashBoard.Common;
using Fliq.Application.Users.Commands;
using Fliq.Application.Users.Queries;
using Fliq.Contracts.Authentication;
using Fliq.Contracts.Common;
using Fliq.Contracts.Users.UserFeatureActivities;
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("delete-user/{userId}")]
        public async Task<IActionResult> DeleteUserById(int userId)
        {
            _logger.LogInfo($"Delete user with ID: {userId} received");

            var command = new DeleteUserByIdCommand(userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Delete user with ID: {userId} executed. Result: {result} ");

            return result.Match(
              deleteUserResult => Ok(new DeleteUserResponse( $"User with ID: {userId} was successfully deleted")),
              errors => Problem(errors)
          );

        }


        [Authorize(Roles = "SuperAdmin,Admin")]
        [HttpPost("deactivate-user/{UserId}")]
        public async Task<IActionResult> DeactivateUser(int UserId)
        {
            _logger.LogInfo($"User Deactivation Request Received for User with Id: {UserId}");

            var AdminUserId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {AdminUserId}");

            var command = new DeactivateUserCommand(UserId, AdminUserId);
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
        [HttpGet("users-export")]
        public async Task<IActionResult> ExportUsersToCsv([FromQuery] int roleId,
                                                    [FromQuery] int pageNumber, [FromQuery] int pageSize,
                                                    [FromQuery] bool exportAsBackgroundTask)
        {
            if (pageSize > 1000) exportAsBackgroundTask = true;

            var adminUserId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {adminUserId}");

            if (exportAsBackgroundTask)
            {
                var result = await _mediator.Send(new ExportUsersToCsvCommand(adminUserId, roleId, pageNumber, pageSize));

                if (result.IsError)
                    return BadRequest(result.FirstError.Description);

                return Accepted(new { message = "Export is being processed. You will be notified when it's ready." });
            }
            else
            {
                var result = await _mediator.Send(new GetPaginatedUsersQuery(adminUserId, roleId, pageNumber, pageSize));

                if (result.IsError)
                    return BadRequest(result.FirstError.Description);

                return Ok(result.Value);
            }
        }

        [HttpGet("get-audit-trails")]
        public async Task<IActionResult> GetPaginatedAuditTrails([FromQuery] string? name, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInfo($"Received request for Get Paginated Audit Trails.");

            var query = new GetPaginatedAuditTrailCommand(pageNumber, pageSize, name);
            var result = await _mediator.Send(query, HttpContext.RequestAborted);

            if (result.IsError)
                return BadRequest(result.FirstError.Description);

            return Ok(result.Value);
        }
    }
}
