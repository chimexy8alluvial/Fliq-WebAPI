using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Entities;
namespace Fliq.Infrastructure.Services
{
    public class OtpService : IOtpService
    {
        private static readonly Random _random = new Random();
        private readonly IOtpRepository _otpRepository;
        public OtpService(IOtpRepository otpRepository)
        {
            _otpRepository = otpRepository;
        }
        private async Task<string> GenerateOtp(int length = 6)
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
            var otp = new OTP { Code =await GenerateOtp(), Email = email, ExpiresAt = DateTime.UtcNow.AddMinutes(10), UserId = userId };
            _otpRepository.Add(otp);
            return otp.Code;
        }

        public async Task<bool> OtpExistAsync(string email, string otp)
        {
            return await _otpRepository.OtpExistAsync(email, otp);
        }
    }
}
