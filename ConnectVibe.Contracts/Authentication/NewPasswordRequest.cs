namespace Fliq.Contracts.Authentication;
public record NewPasswordRequest(
     int Id,
        string ConfirmPassword,
        string NewPassword,
        string Otp
);
