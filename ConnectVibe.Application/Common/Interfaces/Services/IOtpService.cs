namespace ConnectVibe.Application.Common.Interfaces.Services
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
        Task<bool> ValidateOtpAsync(string email, string otp);
        Task<string> GetOtpAsync(string email, int userId);
    }
}
