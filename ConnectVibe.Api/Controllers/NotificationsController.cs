using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Notifications.Commands.DeviceRegistration;
using Fliq.Contracts.Notifications.DeviceRegistration;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;
        public NotificationsController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("save-device-token")]
        public async Task<IActionResult> SaveDeviceToken([FromBody] SaveDeviceTokenRequest request)
        {
            _logger.LogInfo($"Device token registration request received: {request}");

            var command = new SaveDeviceTokenCommand(request.UserId, request.DeviceToken);
            var result = await _mediator.Send(command);
            _logger.LogInfo($"Device token registration executed. Result: {result}");

            return result.Match(
                    result => Ok(new SaveDeviceTokenResponse(result.IsSuccess, result.Message)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );
        }

    }
}
