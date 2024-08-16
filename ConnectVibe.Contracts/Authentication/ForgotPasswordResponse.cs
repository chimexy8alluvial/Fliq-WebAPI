namespace ConnectVibe.Contracts.Authentication
{
    public record ForgotPasswordResponse(
      string otp,
      string email
    );
}
