using Fliq.Application.Authentication.Commands.CreateAdmin;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Users.Commands;
using Fliq.Contracts.Authentication;
using Fliq.Contracts.Common;
using Fliq.Contracts.Users;
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


        [Authorize(Roles = "SuperAdmin")]
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
    }
}
