using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.DatingEnvironment.Commands.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Application.DatingEnvironment.Common;
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
    }
}
