using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Explore.Common;
using Fliq.Application.Explore.Queries;
using Fliq.Contracts.Explore;
using Fliq.Contracts.Payments;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExploreController : ControllerBase
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ExploreController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Explore([FromQuery] ExploreRequest request)
        {
            var query = _mapper.Map<ExploreQuery>(request);
            var result = await _mediator.Send(query);

              return result.Match(
                    result => Ok(_mapper.Map<ExploreResponse>(result)),
                    errors => Problem(string.Join("; ", errors.Select(e => e.Description)))
                );

        }

    }
}
