﻿using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services.ImageServices;
using Fliq.Application.MatchedProfile.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.MatchedProfile;
using Fliq.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace Fliq.Application.MatchedProfile.Commands.AcceptedMatch
{
    public class AcceptMatchRequestCommand : IRequest<ErrorOr<CreateAcceptMatchResult>>
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        //public int MatchInitiatorUserId { get; set; }
    }

    public class AcceptMatchRequestCommandHandler : IRequestHandler<AcceptMatchRequestCommand, ErrorOr<CreateAcceptMatchResult>>
    {
        private readonly IMapper _mapper;
        private readonly IImageService _imageService;
        private readonly IUserRepository _userRepository;
        private readonly IMatchProfileRepository _matchProfileRepository;

        public AcceptMatchRequestCommandHandler(IMapper mapper, IImageService imageService, IUserRepository userRepository, IMatchProfileRepository matchProfileRepository)
        {
            _mapper = mapper;
            _imageService = imageService;
            _userRepository = userRepository;
            _matchProfileRepository = matchProfileRepository;
        }

        public async Task<ErrorOr<CreateAcceptMatchResult>> Handle(AcceptMatchRequestCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var acceptorUser = _userRepository.GetUserById(command.UserId);
            //var matchInitiatorUser = _userRepository.GetUserById(command.MatchInitiatorUserId);

            //command.UserId = acceptorUser.Id;
            
            if (acceptorUser == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var matchProfile = _matchProfileRepository.GetMatchProfileById(command.Id);
            matchProfile.matchRequestStatus = MatchRequestStatus.Accepted;

            //matchAcceptorProfile.PictureUrl = matchAcceptorProfile.PictureUrl == null ? "" : matchInitiatorUser.UserProfile.Photos.First().PictureUrl;
            //matchAcceptorProfile.Name = matchInitiatorUser.FirstName;

            _matchProfileRepository.Update(matchProfile);
            return new CreateAcceptMatchResult(matchProfile);

        }
    }
}
