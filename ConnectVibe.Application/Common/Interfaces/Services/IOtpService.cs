namespace Fliq.Application.Common.Interfaces.Services
{
    public interface IOtpService
    {
      
        Task<bool> ValidateOtpAsync(string email, string otp);
        Task<bool> OtpExistAsync(string email, string otp);
        Task<string> GetOtpAsync(string email, int userId);
    }
}
