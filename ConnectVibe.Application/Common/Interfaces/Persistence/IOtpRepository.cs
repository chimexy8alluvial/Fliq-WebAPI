using Fliq.Domain.Entities;

namespace Fliq.Application.Common.Interfaces.Persistence
{
    public interface IOtpRepository
    {
        void Add(OTP otp);
        Task<bool> CheckActiveOtpAsync(string email, string code);
        Task<bool> OtpExistAsync(string email, string code);
    }
}
