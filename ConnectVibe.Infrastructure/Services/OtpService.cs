using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Domain.Entities;
namespace ConnectVibe.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private static readonly Random _random = new Random();
        private readonly IOtpRepository _otpRepository;
        public OtpService(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }
        public string GenerateOtp(int length = 6)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }

        public async Task<bool> ValidateOtpAsync(string email, string otp)
        {
            return await _otpRepository.CheckActiveOtpAsync(email, otp);
        }
        public async Task<string> GetOtpAsync(string email, int userId)
        {
            var otp = new OTP { Code = GenerateOtp(), Email = email, ExpiresAt = DateTime.UtcNow.AddMinutes(10), UserId = userId };
            _otpRepository.Add(otp);
            return otp.Code;
        }

    }
}
