namespace ConnectVibe.Application.Common.Interfaces.Services
{
    public interface IOtpService
    {
        string GenerateOtp(int length = 6);
    }
}
