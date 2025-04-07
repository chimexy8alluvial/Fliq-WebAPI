
using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.PlatformCompliance.Common;
using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.PlatformCompliance.Commands
{
    public record CreateComplianceCommand(
    ComplianceType ComplianceType,
    Language Language,
    string Description,
    string VersionNumber,
    DateTime EffectiveDate,
    bool OptIn
    ) : IRequest<ErrorOr<CreateComplianceResult>>;

    public class CreateComplianceCommandHandler : IRequestHandler<CreateComplianceCommand, ErrorOr<CreateComplianceResult>>
    {
        private readonly IComplianceRepository _complianceRepository;
        private readonly ILoggerManager _logger;

        public CreateComplianceCommandHandler(IComplianceRepository complianceRepository, ILoggerManager logger)
        {
            _complianceRepository = complianceRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateComplianceResult>> Handle(CreateComplianceCommand request, CancellationToken cancellationToken)
        {
            var compliance = new Compliance
            {
                ComplianceType = request.ComplianceType,
                Language = request.Language,
                Description = request.Description,
                VersionNumber = request.VersionNumber,
                EffectiveDate = request.EffectiveDate,
                OptIn = request.OptIn
            };

            await _complianceRepository.AddAsync(compliance); // Assuming you have an AddAsync method
            _logger.LogInfo($"Compliance policy created with ID: {compliance.Id}");

            return new CreateComplianceResult(compliance.Id);
        }
    }
}
