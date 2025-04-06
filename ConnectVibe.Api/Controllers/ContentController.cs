using ErrorOr;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Contents.Commands;
using Fliq.Application.DatingEnvironment.Commands.SpeedDating;
using Fliq.Domain.Entities.DatingEnvironment.BlindDates;
using Fliq.Domain.Entities.DatingEnvironment.SpeedDates;
using Fliq.Domain.Entities.Games;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

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
        [HttpPut("approve/{contentType}/{contentId}")]
        public async Task<IActionResult> ApproveContent(ContentTypeEnum contentType, int contentId)
        {
            _logger.LogInfo($"Approve {contentType} with ID: {contentId} received");
           
            var adminUserId = GetAuthUserId();

            ErrorOr<Unit> result = new();
           
            switch (contentType)
            {
                case ContentTypeEnum.SpeedDate:
                    result = await _mediator.Send(new ApproveContentCommand<SpeedDatingEvent>(contentId, adminUserId));
                    break;
                case ContentTypeEnum.BlindDate:
                    result = await _mediator.Send(new ApproveContentCommand<BlindDate>(contentId, adminUserId));
                    break;
                case ContentTypeEnum.Game:
                    result = await _mediator.Send(new ApproveContentCommand<Game>(contentId, adminUserId));
                    break;
                //Add more cases for other content types

                default:
                    return BadRequest("Invalid content type");
            }

            _logger.LogInfo($"Approve {contentType} with ID: {contentId} executed. Result: {result} ");

            return result.Match(
             cancelEventResult => Ok(new { Message = $"{contentType} request with ID: {contentId} was successfully approved" }),
             errors => Problem(errors)
         );
        }
    }
}
