using Fliq.Application.Authentication.Commands.CreateAdmin;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DashBoard.Command.DeleteUser;
using Fliq.Application.DashBoard.Common;
using Fliq.Contracts.Authentication;
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
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
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
            _logger.LogInfo($"Delete user with Id {userId} received");

            var command = new DeleteUserByIdCommand(userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Delete user with Id {userId} executed. Result: {result} ");

            return result.Match(
              deleteUserResult => Ok(new DeleteUserResponse( $"User with ID {userId} successfully deleted")),
              errors => Problem(errors)
          );

        }
    }
}
