
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Fliq.Application.PlatformCompliance.Queries
{
    public record GetUserConsentHistoryQuery(
    int UserId,
    ComplianceType? ComplianceType = null
    ) : IRequest<ErrorOr<List<UserConsentHistoryDto>>>;

    public class UserConsentHistoryDto
    {
        public int Id { get; set; }
        public DateTime ConsentDate { get; set; }
        public bool OptIn { get; set; }
        public string IPAddress { get; set; } = string.Empty;
        public ComplianceType ComplianceType { get; set; }
        public string VersionNumber { get; set; } = string.Empty;
    }

    public class GetUserConsentHistoryQueryHandler : IRequestHandler<GetUserConsentHistoryQuery, ErrorOr<List<UserConsentHistoryDto>>>
    {
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserRepository _userRepository;

        public GetUserConsentHistoryQueryHandler(
            IUserConsentRepository userConsentRepository,
            IUserRepository userRepository)
        {
            _userConsentRepository = userConsentRepository;
            _userRepository = userRepository;
        }

        public async Task<ErrorOr<List<UserConsentHistoryDto>>> Handle(GetUserConsentHistoryQuery request, CancellationToken cancellationToken)
        {
            var user =  _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                return Errors.User.UserNotFound;
            }

            var consentHistory = await _userConsentRepository.GetUserConsentsHistoryAsync(
                request.UserId, request.ComplianceType);

            return consentHistory.Select(c => new UserConsentHistoryDto
            {
                Id = c.Id,
                ConsentDate = c.DateCreated,
                OptIn = c.OptIn,
                IPAddress = c.IPAddress,
                ComplianceType = c.Compliance.ComplianceType,
                VersionNumber = c.Compliance.VersionNumber
            }).ToList();
        }
    }
}
