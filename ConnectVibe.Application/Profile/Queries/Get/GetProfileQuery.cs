using ErrorOr;
using Fliq.Application.Authentication.Common.Profile;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Common.Errors;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Fliq.Application.Profile.Queries.Get
{
    public record GetProfileQuery() : IRequest<ErrorOr<CreateProfileResult>>;

    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ErrorOr<CreateProfileResult>>
    {
        private readonly IProfileRepository _profileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private const int UnauthorizedUserId = -1;

        public GetProfileQueryHandler(IProfileRepository profileRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _profileRepository = profileRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ErrorOr<CreateProfileResult>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            var userId = GetUserId();

            var user = _userRepository.GetUserById(userId);
            if (user == null)
            {
                return Errors.Profile.ProfileNotFound;
            }
            var userProfile = _profileRepository.GetProfileByUserId(userId);
            if (userProfile == null)
            {
                return Errors.Profile.ProfileNotFound;
            }

            return new CreateProfileResult(userProfile);
        }

        private int GetUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return int.TryParse(userIdClaim, out int userId) ? userId : UnauthorizedUserId;
        }
    }
}