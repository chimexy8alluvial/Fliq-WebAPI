using Fliq.Application.BusinessDocumentType.Command;
using Fliq.Application.BusinessDocumentType.Common;
using Fliq.Application.Common.Interfaces.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MapsterMapper;
using Fliq.Application.BusinessDocumentType.Query;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessDocumentTypeController : ApiBaseController
    {
        private readonly IMediator _mediator;
        private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;
        public BusinessDocumentTypeController(IMediator mediator, ILoggerManager logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("add-business-document-type")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> AddBusinessDocumentType([FromBody] BusinessDocumentTypeRequest request)
        {
            _logger.LogInfo($"Adding document type: {request.Name}");

            var command = _mapper.Map<AddBusinessDocumentTypeCommand>(request);

            var result = await _mediator.Send(command);

            return result.Match(
                documentType => CreatedAtAction(nameof(GetAllBusinessDocumentTypes), null, documentType),
                errors => Problem(errors));
        }

        [HttpGet("get-all-business-document-types")]
        public async Task<IActionResult> GetAllBusinessDocumentTypes()
        {
            _logger.LogInfo("Retrieving all business document types");

            var result = await _mediator.Send(new GetAllBusinessDocumentTypesQuery());

            return result.Match(
                documentTypes => Ok(documentTypes),
                errors => Problem(errors));
        }

        [HttpGet("get-business-document-type-by-id{id}")]
        public async Task<IActionResult> GetBusinessDocumentTypeById(int id, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Retrieving document type with ID: {id}");
            var result = await _mediator.Send(new GetBusinessDocumentTypeByIdQuery(id), cancellationToken);
            return result.Match(
                documentType => Ok(documentType),
                errors => Problem(errors));
        }

        [HttpPut("delete-business-document-type{id}")]
        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            _logger.LogInfo($"Deleting document type with ID: {id}");
            var result = await _mediator.Send(new DeleteBusinessDocumentTypeCommand(id), cancellationToken);
            return result.Match(
                _ => NoContent(),
                errors => Problem(errors));
        }
    }
}