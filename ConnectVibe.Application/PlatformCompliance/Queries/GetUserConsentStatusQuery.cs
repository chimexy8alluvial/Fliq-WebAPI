
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.PlatformCompliance.Common;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.PlatformCompliance.Queries
{
    public record GetUserConsentStatusQuery(
        int UserId,
        int ComplianceTypeId
    ) : IRequest<ErrorOr<UserConsentStatusResult>>;

    public class GetUserConsentStatusQueryHandler : IRequestHandler<GetUserConsentStatusQuery, ErrorOr<UserConsentStatusResult>>
    {
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IComplianceRepository _complianceRepository;

        public GetUserConsentStatusQueryHandler(
            IUserConsentRepository userConsentRepository,
            IComplianceRepository complianceRepository)
        {
            _userConsentRepository = userConsentRepository;
            _complianceRepository = complianceRepository;
        }

        public async Task<ErrorOr<UserConsentStatusResult>> Handle(GetUserConsentStatusQuery request, CancellationToken cancellationToken)
        {
            // Get the latest version of this compliance type
            var latestCompliance = await _complianceRepository.GetLatestComplianceByTypeAsync(request.ComplianceTypeId);
            if (latestCompliance == null)
            {
                return Errors.Compliance.ComplianceNotFound;
            }

            // Get the user's most recent consent for this compliance type
            var userConsent = await _userConsentRepository.GetUserConsentForComplianceTypeAsync(
                request.UserId, request.ComplianceTypeId);

            // If no consent found, they need to review
            if (userConsent == null)
            {
                return new UserConsentStatusResult
                {
                    HasConsented = false,
                    NeedsReview = true
                };
            }

            // If consented to an older version, they need to review again
            bool needsReview = userConsent.Compliance.VersionNumber != latestCompliance.VersionNumber;

            return new UserConsentStatusResult
            {
                HasConsented = userConsent.OptIn,
                ConsentDate = userConsent.DateCreated,
                VersionNumber = userConsent.Compliance.VersionNumber,
                NeedsReview = needsReview
            };
        }
    }
}
