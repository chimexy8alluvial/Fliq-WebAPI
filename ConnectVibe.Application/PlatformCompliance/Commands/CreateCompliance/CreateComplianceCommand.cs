using ErrorOr;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.PlatformCompliance.Common;
using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;
using MediatR;

namespace Fliq.Application.PlatformCompliance.Commands.CreateCompliance
{
    public record CreateComplianceCommand(
     int ComplianceTypeId,
    Language Language,
    string Description,
    string VersionNumber,
    DateTime EffectiveDate
    ) : IRequest<ErrorOr<CreateComplianceResult>>;

    public class CreateComplianceCommandHandler : IRequestHandler<CreateComplianceCommand, ErrorOr<CreateComplianceResult>>
    {
        private readonly IComplianceRepository _complianceRepository;
        private readonly IComplianceTypeRepository _complianceTypeRepository;
        private readonly ILoggerManager _logger;

        public CreateComplianceCommandHandler(IComplianceRepository complianceRepository, IComplianceTypeRepository complianceTypeRepository, ILoggerManager logger)
        {
            _complianceRepository = complianceRepository;
            _complianceTypeRepository = complianceTypeRepository;
            _logger = logger;
        }

        public async Task<ErrorOr<CreateComplianceResult>> Handle(CreateComplianceCommand request, CancellationToken cancellationToken)
        {
            var complianceType = await _complianceTypeRepository.GetByIdAsync(request.ComplianceTypeId);
            if (complianceType == null)
            {
                _logger.LogError($"Compliance type with ID {request.ComplianceTypeId} not found.");
                return Error.NotFound("ComplianceType.NotFound", $"Compliance type with ID {request.ComplianceTypeId} not found.");
            }
            var compliance = new Compliance
            {
                ComplianceTypeId = request.ComplianceTypeId,
                Language = request.Language,
                Description = request.Description,
                VersionNumber = request.VersionNumber,
                EffectiveDate = request.EffectiveDate,
            };

            await _complianceRepository.AddAsync(compliance);
            _logger.LogInfo($"Compliance policy created with ID: {compliance.Id}");

            return new CreateComplianceResult(compliance.Id);
        }
    }
}
