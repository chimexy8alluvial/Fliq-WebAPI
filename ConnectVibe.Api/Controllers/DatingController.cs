using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Pagination;
using Fliq.Application.DatingEnvironment.Commands.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Commands.BlindDates;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Application.DatingEnvironment.Common;
using Fliq.Application.DatingEnvironment.Queries.BlindDateCategory;
using Fliq.Application.DatingEnvironment.Queries.BlindDates;
using Fliq.Application.DatingEnvironment.Queries.SpeedDates;
using Fliq.Application.HelpAndSupport.Queries.GetSupportTickets;
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
                ? new DatePhotoMapped ( request.BlindDateImage.DateSessionImageFile): null
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
                result => Ok(new JoinBlindDateResponse($"User {userId} joined session at {DateTime.UtcNow}")),
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

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("approve-blind-date/{blindDateId}")]
        public async Task<IActionResult> ApproveBlindDateById(int blindDateId)
        {
            _logger.LogInfo($"Approve blind date with ID: {blindDateId} received");
            var userId = GetAuthUserId();

            var command = new ApproveBlindDateCommand(blindDateId, userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Approve blind date with ID: {blindDateId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok($"Blind date request with ID: {blindDateId} was successfully approved"),
              errors => Problem(errors)
          );
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("reject-blind-date/{blindDateId}")]
        public async Task<IActionResult> RejectBlindDateById(int blindDateId)
        {
            _logger.LogInfo($"Reject blind date with ID: {blindDateId} received");
            var userId = GetAuthUserId();

            var command = new RejectBlindDateCommand(blindDateId, userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Reject blind date with ID: {blindDateId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok($"Blind date request with ID: {blindDateId} was successfully approved"),
              errors => Problem(errors)
          );
        }


        [HttpGet]
        public async Task<IActionResult> GetPaginatedBlindDatesForAdmin(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] int? CreationStatus = null)
        {
            var query = new GetPaginatedBlindDatesForAdminQuery(pageNumber, pageSize, CreationStatus);

            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return Ok(result.Value);
        }

        //-------speed--dating------\\

        [HttpPost("SpeedDate")] 
        [Produces(typeof(CreateSpeedDatingEventResponse))]
        public async Task<IActionResult> CreateSpeedDateEvent([FromForm] CreateSpeedDatingEventRequest request)
        {
            _logger.LogInfo($"Create Speed Dating request received: {request}");
            var userId = GetAuthUserId();
            _logger.LogInfo($"Authenticated user ID: {userId}");

            var command = _mapper.Map<CreateSpeedDatingEventCommand>(request) with
            {
                SpeedDateImage = request.SpeedDatingImage is not null
                ? new DatePhotoMapped(request.SpeedDatingImage.DateSessionImageFile) : null
            };

            var result = await _mediator.Send(command);
            _logger.LogInfo($"Create Speed Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<CreateSpeedDatingEventResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("SpeedDate/start")]
        [Produces(typeof(StartSpeedDatingEventResponse))]
        [Authorize]
        public async Task<IActionResult> StartSpeedDate([FromBody] StartSpeedDatingEventRequest request)
        {
            _logger.LogInfo($"Start Speed Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new StartSpeedDatingEventCommand(userId, request.SpeedDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Start Speed Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<StartSpeedDatingEventResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [HttpPost("SpeedDate/join")]
        [Produces(typeof(JoinSpeedDatingEventResponse))]
        [Authorize]
        public async Task<IActionResult> JoinSpeedDate([FromBody] JoinSpeedDatingEventRequest request)
        {
            _logger.LogInfo($"Join Speed Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new JoinSpeedDatingEventCommand(userId, request.SpeedDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Join Speed Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(new JoinSpeedDatingEventResponse($"User {userId} joined session at {DateTime.UtcNow}")),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }


        [HttpPost("SpeedDate/end")]
        [Produces(typeof(EndSpeedDatingEventResponse))]
        [Authorize]
        public async Task<IActionResult> EndSpeedDate([FromBody] EndSpeedDatingEventRequest request)
        {
            _logger.LogInfo($"End Speed Date request received: {request}");
            var userId = GetAuthUserId();

            var command = new EndSpeedDatingEventCommand(userId, request.SpeedDateId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"End Speed Date Command Executed. Result: {result}");

            return result.Match(
                result => Ok(_mapper.Map<EndSpeedDatingEventResponse>(result)),
                errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
            );
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("approve-speed-date/{speedDateId}")]
        public async Task<IActionResult> ApproveSpeedDateById(int speedDateId)
        {
            _logger.LogInfo($"Approve speed date with ID: {speedDateId} received");
            var userId = GetAuthUserId();

            var command = new ApproveSpeedDateCommand(speedDateId, userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Approve speed date with ID: {speedDateId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok($"Speed date request with ID: {speedDateId} was successfully approved"),
              errors => Problem(errors)
          );
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("reject-speed-date/{speedDateId}")]
        public async Task<IActionResult> RejectSpeedDateById(int speedDateId)
        {
            _logger.LogInfo($"Reject speed date with ID: {speedDateId} received");
            var userId = GetAuthUserId();

            var command = new RejectSpeedDateCommand(speedDateId, userId);
            var result = await _mediator.Send(command);

            _logger.LogInfo($"Reject speed date with ID: {speedDateId} executed. Result: {result} ");

            return result.Match(
              cancelEventResult => Ok($"Speed date request with ID: {speedDateId} was successfully approved"),
              errors => Problem(errors)
          );
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginatedSpeedDatesForAdmin(
           [FromQuery] int pageNumber = 1,
           [FromQuery] int pageSize = 10,
           [FromQuery] int? CreationStatus = null)
        {
            var query = new GetPaginatedSpeedDatesForAdminQuery(pageNumber, pageSize, CreationStatus);

            var result = await _mediator.Send(query);

            if (result.IsError)
            {
                return BadRequest(result.FirstError.Description);
            }

            return Ok(result.Value);
        }
    }
}
