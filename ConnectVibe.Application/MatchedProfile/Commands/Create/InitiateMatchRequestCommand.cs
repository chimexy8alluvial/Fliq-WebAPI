﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Application.Profile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.Create
{
    public class InitiateMatchRequestCommand : IRequest<ErrorOr<CreateMatchProfileResult>>
    {
        public int UserId { get; set; }
        public int MatchInitiatorUserId { get; set; }
    }

    public class InitiateMatchRequestCommandHandler : IRequestHandler<InitiateMatchRequestCommand, ErrorOr<CreateMatchProfileResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;

        public InitiateMatchRequestCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<CreateMatchProfileResult>> Handle(InitiateMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var requestedUser = _userRepository.GetUserById(command.UserId);
            if (requestedUser == null)
            {
                return Errors.User.UserNotFound;
            }
            var matchInitiatorUser = _userRepository.GetUserById(command.MatchInitiatorUserId);
            if (matchInitiatorUser == null)
            {
                return Errors.User.UserNotFound;
            }

            command.UserId = requestedUser.Id;
            var matchProfile = _mapper.Map<MatchRequest>(command);
           
            matchProfile.matchRequestStatus = MatchRequestStatus.Pending;
            //matchProfile.PictureUrl = matchInitiatorUser.UserProfile.Photos.First().PictureUrl;
            matchProfile.PictureUrl = matchProfile.PictureUrl == null ? "" : matchInitiatorUser.UserProfile.Photos.First().PictureUrl;
            matchProfile.Name = matchInitiatorUser.FirstName;
            

            _matchProfileRepository.Add(matchProfile);

            return new CreateMatchProfileResult(matchProfile.MatchInitiatorUserId,
                matchProfile.Name,
                matchProfile.PictureUrl);
        }
    }
}
