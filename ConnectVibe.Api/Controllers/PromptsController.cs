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

        [Obsolete]
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

        [Obsolete]
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

        [HttpPost("AddCategory")]
        [Produces(typeof(AddPromptCategoryResponse))]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddCategory([FromBody] AddPromptCategoryRequest request)
        {
            _logger.LogInfo($"Add Prompt Category  request received: {request}");
            var command = new AddPromptCategoryCommand(request.CategoryName);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Add Prompt Category Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<AddPromptCategoryResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("AddSystemPrompt")]
        [Produces(typeof(AddSystemPromptResponse))]
        [AllowAnonymous]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddSystemPrompt([FromBody] AddSystemPromptRequest request)
        {
            _logger.LogInfo($"Add System Prompt request received: {request}");
            var command = new AddSystemPromptCommand(request.QuestionText, request.CategoryId);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Add System Prompt Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<AddSystemPromptResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }
    }
}
