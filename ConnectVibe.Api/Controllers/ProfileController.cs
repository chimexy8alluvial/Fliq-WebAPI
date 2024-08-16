using ConnectVibe.Application.Authentication.Common.Profile;
using ConnectVibe.Application.Profile.Commands.Create;
using ConnectVibe.Contracts.Profile;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ConnectVibe.Api.Controllers
{
    [Route("api/profile")]
    [AllowAnonymous]
    public class ProfileController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public ProfileController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProfileRequest request)
        {
            //var userId = GetAuthUserId();
            var command = _mapper.Map<CreateProfileCommand>(request);
            command.Photos = request.Photos;
            var profileResult = await _mediator.Send(command);

            return profileResult.Match(
                profileResult => Ok(_mapper.Map<CreateProfileResult>(profileResult)),
                errors => Problem(errors)
                );
        }
    }
}