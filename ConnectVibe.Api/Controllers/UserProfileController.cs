﻿using Fliq.Application.Common.Interfaces.Services;
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
        private readonly ILoggerManager _logger;

        public UserProfileController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInfo("Get Profile query recieved");
            var userId = GetAuthUserId();
            var query = new GetProfileQuery(userId);
            var profileResult = await _mediator.Send(query);

            _logger.LogInfo($"Profile: {profileResult}");
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
            _logger.LogInfo($"Update Profile Request recieved");
            var userId = GetAuthUserId();

            var modifiedRequest = request with { UserId = userId };
            var command = _mapper.Map<UpdateProfileCommand>(modifiedRequest);
            if (request.Photos != null)
            {
                command.Photos = request.Photos.Select(photo => new ProfilePhotoMapped
                {
                    Caption = photo.Caption,
                    ImageFile = photo.ImageFile
                }).ToList();
            }
            var profileResult = await _mediator.Send(command);
            _logger.LogInfo($"Updated profile: {profileResult}");

            return profileResult.Match(
                profileResult => Ok(_mapper.Map<UpdateProfileResponse>(profileResult)),
                errors => Problem(errors)
                );
        }
    }
}