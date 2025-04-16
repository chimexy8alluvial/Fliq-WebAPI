using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using Fliq.Domain.Entities.PlatformCompliance;
using MediatR;

namespace Fliq.Application.PlatformCompliance.Commands
{
    public record RecordUserConsentCommand(
       int UserId,
       int ComplianceId,
       bool OptIn,
       string IPAddress
    ) : IRequest<ErrorOr<Unit>>;

    public class RecordUserConsentCommandHandler : IRequestHandler<RecordUserConsentCommand, ErrorOr<Unit>>
    {
        private readonly IUserConsentRepository _userConsentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IComplianceRepository _complianceRepository;
        private readonly ILoggerManager _logger;

        public RecordUserConsentCommandHandler(
            IUserConsentRepository userConsentRepository,
            IUserRepository userRepository,
            IComplianceRepository complianceRepository,
            ILoggerManager logger)
        {
            _userConsentRepository = userConsentRepository;
            _userRepository = userRepository;
            _complianceRepository = complianceRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<Unit>> Handle(RecordUserConsentCommand request, CancellationToken cancellationToken)
        {
            var user = _userRepository.GetUserById(request.UserId);
            if (user == null)
            {
                _logger.LogError($"User with ID {request.UserId} not found");
                return Errors.User.UserNotFound;
            }

            var compliance = await _complianceRepository.GetByIdAsync(request.ComplianceId);
            if (compliance == null)
            {
                _logger.LogError($"Compliance with ID {request.ComplianceId} not found");
                return Errors.Compliance.ComplianceNotFound;
            }

            // Check if there's an existing consent record
            var existingConsent = await _userConsentRepository.GetUserConsentForComplianceAsync(request.UserId, request.ComplianceId);

            if (existingConsent != null)
            {
                // Update existing consent record
                existingConsent.OptIn = request.OptIn;
                existingConsent.IPAddress = request.IPAddress;
                existingConsent.DateModified = DateTime.UtcNow;

                await _userConsentRepository.UpdateAsync(existingConsent);
                _logger.LogInfo($"Updated consent record for User {request.UserId} on Compliance {request.ComplianceId}");
            }
            else
            {
                // Create new consent record
                var userConsent = new UserConsentAction
                {
                    UserId = request.UserId,
                    ComplianceId = request.ComplianceId,
                    OptIn = request.OptIn,
                    IPAddress = request.IPAddress
                };

                await _userConsentRepository.AddAsync(userConsent);
                _logger.LogInfo($"Created new consent record for User {request.UserId} on Compliance {request.ComplianceId}");
            }

            return Unit.Value;
        }
    }
}
