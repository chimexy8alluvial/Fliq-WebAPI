﻿using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using MediatR;

namespace Fliq.Application.Profile.Queries.Get
{
    public record GetProfileQuery(int UserId) : IRequest<ErrorOr<CreateProfileResult>>;

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ErrorOr<CreateProfileResult>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILoggerManager _logger;

        public GetProfileQueryHandler(IProfileRepository profileRepository, IUserRepository userRepository, ILoggerManager logger)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with id {request.UserId} not found.");
                return Errors.Profile.ProfileNotFound;
            }
            var userProfile = _profileRepository.GetProfileByUserId(request.UserId);
            if (userProfile == null)
            {
                _logger.LogError($"Profile for user with id {request.UserId} not found.");
                return Errors.Profile.ProfileNotFound;
            }

            return new CreateProfileResult(userProfile);
        }
    }
}