using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Prompts.Commands;
using Fliq.Contracts.Prompts;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PromptsController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public PromptsController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("Prompt-Answer")]
        [Produces(typeof(CreatePromptAnswerResponse))]
        public async Task<IActionResult> CreatePromptAnswer([FromForm] CreatePromptAnswerRequest request)
        {
            _logger.LogInfo($"Prompt answer request received: {request}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            // Map request to command and add UserId
            var command = _mapper.Map<PromptAnswerCommand>(request);
            command = command with { UserId = userId};

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Prompt Answer Command Executed. Result: {result}");

            return result.Match(
                    result => Ok(_mapper.Map<CreatePromptAnswerResponse>(result)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );

        }

        [HttpPost("Add-custom")]
        [Produces(typeof(CreateCustomPromptResponse))]
        public async Task<IActionResult> CreateCustomPrompt([FromForm] CreateCustomPromptRequest request)
        {
            _logger.LogInfo($"Custom Prompt request received: {request}");

            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            // Map request to command and add UserId
            var command = _mapper.Map<CreateCustomPromptCommand>(request);
            command = command with { UserId = userId };

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Custom Prompt Command Executed. Result: {result}");

            return result.Match(
                    result => Ok(_mapper.Map<CreateCustomPromptResponse>(result)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );

        }
    }
}
