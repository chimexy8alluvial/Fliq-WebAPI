using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Common.Interfaces.Persistence
{
    public interface IOtpRepository
    {
        void Add(OTP otp);
        bool CheckActiveOtp(string email, string code);
    }
}
