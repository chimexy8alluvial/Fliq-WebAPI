﻿using Fliq.Application.Profile.Commands.Create;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Profile;
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
    }
}