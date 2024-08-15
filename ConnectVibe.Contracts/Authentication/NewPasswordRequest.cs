namespace ConnectVibe.Contracts.Authentication;
public record NewPasswordRequest(
    string Email,
    string ConfirmPassword,
    string NewPassword
);
