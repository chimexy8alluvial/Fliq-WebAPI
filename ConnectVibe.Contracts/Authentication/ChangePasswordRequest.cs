namespace ConnectVibe.Contracts.Authentication;
public record ChangePasswordRequest(
    string Email,
    string OldPassword,
    string NewPassword
    );
