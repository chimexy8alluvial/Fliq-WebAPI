using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.LiveComms.Commands.Chats;
using Fliq.Contracts.LiveComms.Chats;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LiveCommsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public LiveCommsController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("start-chat")]
        public async Task<IActionResult> StartChat([FromBody] CreateChatChannelRequest request)
        {
            _logger.LogInfo($"Create Chat Channel request received: {request}");
            var command = new CreateChatChannelCommand(request.UserId1, request.UserId2);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Create Chat Channel Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<CreateChatChannelResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }
    }
}
