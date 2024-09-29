using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Commands.Update;
using Fliq.Application.Profile.Common;
using Fliq.Application.Profile.Queries.Get;
using Fliq.Contracts.Profile;
using Fliq.Contracts.Profile.UpdateDtos;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Fliq.Api.Controllers
{
    [Route("api/user-profile")]
    public class UserProfileController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;

        public UserProfileController(ISender mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var query = new GetProfileQuery();
            var profileResult = await _mediator.Send(query);

            return profileResult.Match(
                profileResult => Ok(_mapper.Map<UpdateProfileResponse>(profileResult)),
                errors => Problem(errors)
                );
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Create([FromForm] CreateProfileRequest request)
        {
            var command = _mapper.Map<CreateProfileCommand>(request);
            command.Photos = request.Photos.Select(photo => new ProfilePhotoMapped
            {
                Caption = photo.Caption,
                ImageFile = photo.ImageFile
            }).ToList();
            var profileResult = await _mediator.Send(command);

            return profileResult.Match(
                profileResult => Ok(_mapper.Map<ProfileResponse>(profileResult)),
                errors => Problem(errors)
                );
        }

        [HttpPut("update")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Update([FromForm] UpdateProfileRequest request)
        {
            var command = _mapper.Map<UpdateProfileCommand>(request);
            if (request.Photos != null)
            {
                command.Photos = request.Photos.Select(photo => new ProfilePhotoMapped
                {
                    Caption = photo.Caption,
                    ImageFile = photo.ImageFile
                }).ToList();
            }
            var profileResult = await _mediator.Send(command);

            return profileResult.Match(
                profileResult => Ok(_mapper.Map<UpdateProfileResponse>(profileResult)),
                errors => Problem(errors)
                );
        }
    }
}