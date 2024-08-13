namespace ConnectVibe.Contracts.Authentication;

public record ResetPasswordRequest(

    string Password,
    string ConfirmPassword,
    string Email,
    string Token
    );
