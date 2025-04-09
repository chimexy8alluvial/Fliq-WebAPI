using ErrorOr;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Contents.Commands;
using Fliq.Application.Contents.Queries;
using Fliq.Contracts.Contents;
using Fliq.Contracts.DashBoard;
using Fliq.Contracts.Enums;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Entities.Prompts;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContentController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public ContentController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("approve-content")]
        public async Task<IActionResult> ApproveContent(ApproveContentRequest request)
        {
            _logger.LogInfo($"Approve {request.ContentType} with ID: {request.ContentId} received");
           
            var adminUserId = GetAuthUserId();

            ErrorOr<Unit> result = new();
           
            switch (request.ContentType)
            {
                case ContentTypeEnum.SpeedDate:
                    result = await _mediator.Send(new ApproveContentCommand<SpeedDatingEvent>(request.ContentId, adminUserId));
                    break;
                case ContentTypeEnum.BlindDate:
                    result = await _mediator.Send(new ApproveContentCommand<BlindDate>(request.ContentId, adminUserId));
                    break;
                case ContentTypeEnum.Game:
                    result = await _mediator.Send(new ApproveContentCommand<Game>(request.ContentId, adminUserId));
                    break;
                case ContentTypeEnum.Prompt:
                    result = await _mediator.Send(new ApproveContentCommand<PromptQuestion>(request.ContentId, adminUserId));
                    break;
                case ContentTypeEnum.Event:
                    result = await _mediator.Send(new ApproveContentCommand<Events>(request.ContentId, adminUserId));
                    break;
                //Add more cases for other content types

                default:
                    return BadRequest("Invalid content type");
            }

            _logger.LogInfo($"Approve {request.ContentType} with ID: {request.ContentId} executed. Result: {result} ");

            return result.Match(
             cancelEventResult => Ok(new { Message = $"{request.ContentType} request with ID: {request.ContentId} was successfully approved" }),
             errors => Problem(errors)
         );
        }

        [Authorize(Roles = "Admin,SuperAdmin")]
        [HttpPut("reject-content")]
        public async Task<IActionResult> RejectContent(RejectContentRequest request)
        {
            _logger.LogInfo($"Reject {request.ContentType} content with ID: {request.ContentId} received");

            var adminUserId = GetAuthUserId();

            ErrorOr<Unit> result = new();

            switch (request.ContentType)
            {
                case ContentTypeEnum.SpeedDate:
                    result = await _mediator.Send(new RejectContentCommand<SpeedDatingEvent>(request.ContentId, adminUserId, request.RejectionReason));
                    break;
                case ContentTypeEnum.BlindDate:
                    result = await _mediator.Send(new RejectContentCommand<BlindDate>(request.ContentId, adminUserId, request.RejectionReason));
                    break;
                case ContentTypeEnum.Game:
                    result = await _mediator.Send(new RejectContentCommand<Game>(request.ContentId, adminUserId, request.RejectionReason));
                    break;
                case ContentTypeEnum.Prompt:
                    result = await _mediator.Send(new RejectContentCommand<PromptQuestion>(request.ContentId, adminUserId, request.RejectionReason));
                    break;
                case ContentTypeEnum.Event:
                    result = await _mediator.Send(new RejectContentCommand<Events>(request.ContentId, adminUserId, request.RejectionReason));
                    break;
                //Add more cases for other content types

                default:
                    return BadRequest("Invalid content type");
            }

            _logger.LogInfo($"Reject {request.ContentType} with ID: {request.ContentId} executed. Result: {result} ");

            return result.Match(
             cancelEventResult => Ok(new { Message = $"{request.ContentType} request with ID: {request.ContentId} was successfully rejected" }),
             errors => Problem(errors)
         );
        }

        [HttpGet("all-content-count")]
        public async Task<IActionResult> GetContentCount()
        {
            var query = new GetAllContentsCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
               matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
               errors => Problem(errors)
               );
        }

        [HttpGet("flagged-content-count")]
        public async Task<IActionResult> GetFlaggedContentCount()
        {
            var query = new GetAllFlaggedContentsCountQuery();
            var result = await _mediator.Send(query);

            return result.Match(
               matchedProfileResult => Ok(_mapper.Map<UserCountResponse>(result.Value)),
               errors => Problem(errors)
               );
        }
    }
}
