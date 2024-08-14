namespace ConnectVibe.Contracts.Authentication;

public record ForgotPasswordRequest(

    string Password,
    string ConfirmPassword,
    string Email
    );

