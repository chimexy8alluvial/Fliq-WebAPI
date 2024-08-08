using ConnectVibe.Application.Common.Interfaces.Services;
namespace ConnectVibe.Infrastructure.Services
{
    public class OtpService: IOtpService
    {
        private static readonly Random _random = new Random();

        public string GenerateOtp(int length = 6)
        {
            const string chars = "0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}
