using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Application.DatingEnvironment.Queries.BlindDateCategory;
using Fliq.Contracts.Dating;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DatingController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public DatingController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("BlindDateCategory")]
        [Produces(typeof(AddBlindDateCategoryResponse))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddBlindDateCategory([FromBody] AddBlindDateCategoryRequest request)
        {
            _logger.LogInfo($"Add Blind Date Category request received: {request}");
            var command = new AddBlindDateCategoryCommand(request.CategoryName, request.Description);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Add Blind Date Category Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<AddBlindDateCategoryResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPut("UpdateBlindDateCategory")]
        [Produces(typeof(AddBlindDateCategoryResponse))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateBlindDateCategory([FromBody] UpdateBlindDateCategoryRequest request)
        {
            _logger.LogInfo($"Update Blind Date Category request received: {request}");
            var command = new UpdateBlindDateCategoryCommand(request.CategoryId, request.CategoryName, request.Description);

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Update Blind Date Category Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<AddBlindDateCategoryResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }


        [HttpGet("BlindDateCategory/{CategoryId}")]
        [Produces(typeof(GetBlindDateCategoryResponse))]
        public async Task<IActionResult> GetBlindDateCategory(int CategoryId)
        {
            _logger.LogInfo($"Fetch Blind Date Category request received for ID: {CategoryId}");
            var query = new GetBlindDateCategoryByIdQuery(CategoryId);

            var result = await _mediator.Send(query);
            _logger.LogInfo($"Get Blind Date Category query Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<GetBlindDateCategoryResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("BlindDate")]
        [Produces(typeof(CreateBlindDateResponse))]
        public async Task<IActionResult> CreateBlindDate([FromBody] CreateBlindDateRequest request)
        {
            _logger.LogInfo($"Create Blind Date request received: {request}");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var command = _mapper.Map<CreateBlindDateCommand>(request) with
            {
                BlindDateImage = request.BlindDateImage is not null
                ? new BlindDatePhotoMapped ( request.BlindDateImage.BlindDateSessionImageFile): null
            };

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Create Blind Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<CreateBlindDateResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("BlindDate/start")]
        [Produces(typeof(StartBlindDateResponse))]
        [Authorize]
        public async Task<IActionResult> StartBlindDate([FromBody] StartBlindDateRequest request)
        {
            _logger.LogInfo($"Start Blind Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new StartBlindDateCommand(userId, request.BlindDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Start Blind Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<StartBlindDateResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("BlindDate/join")]
        [Produces(typeof(JoinBlindDateResponse))]
        [Authorize]
        public async Task<IActionResult> JoinBlindDate([FromBody] JoinBlindDateRequest request)
        {
            _logger.LogInfo($"Join Blind Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new JoinBlindDateCommand(userId, request.BlindDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Join Blind Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new JoinBlindDateResponse()),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }


        [HttpPost("BlindDate/end")]
        [Produces(typeof(EndBlindDateResponse))]
        [Authorize]
        public async Task<IActionResult> EndBlindDate([FromBody] EndBlindDateRequest request)
        {
            _logger.LogInfo($"End Blind Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new EndBlindDateCommand(userId, request.BlindDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"End Blind Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<EndBlindDateResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

    }
}
