using Fliq.Domain.Entities.PlatformCompliance;
using Fliq.Domain.Enums;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IUserConsentRepository
    {
        Task AddAsync(UserConsentAction userConsent);
        Task UpdateAsync(UserConsentAction userConsent);
        Task<UserConsentAction?> GetUserConsentForComplianceAsync(int userId, int complianceId);
        Task<UserConsentAction?> GetUserConsentForComplianceTypeAsync(int userId, ComplianceType complianceType);
        Task<List<UserConsentAction>> GetUserConsentsHistoryAsync(int userId, ComplianceType? complianceType = null);
    }
}
