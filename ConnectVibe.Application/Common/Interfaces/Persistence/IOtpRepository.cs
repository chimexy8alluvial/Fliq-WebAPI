using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Common.Interfaces.Persistence
{
    public interface IOtpRepository
    {
        void Add(OTP otp);
        Task<bool> CheckActiveOtpAsync(string email, string code);
        Task<bool> OtpExistAsync(string email, string code);
    }
}
