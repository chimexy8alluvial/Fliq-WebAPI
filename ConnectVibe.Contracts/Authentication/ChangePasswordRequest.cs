namespace Fliq.Contracts.Authentication;
public record ChangePasswordRequest(
    string Email,
    string OldPassword,
    string NewPassword
    );
