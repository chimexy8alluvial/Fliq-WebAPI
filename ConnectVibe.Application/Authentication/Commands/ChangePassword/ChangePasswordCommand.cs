using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Security;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MediatR;

namespace Fliq.Application.Authentication.Commands.ChangePassword
{
    public record ChangePasswordCommand(
        string Email,
        string OldPassword,
        string NewPassword
        ) : IRequest<ErrorOr<bool>>;

    public class ChangePasswordQueryHandler : IRequestHandler<ChangePasswordCommand, ErrorOr<bool>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly ILoggerManager _logger;

        public ChangePasswordQueryHandler(IUserRepository userRepository, IEmailService emailService, ILoggerManager logger)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<ErrorOr<bool>> Handle(ChangePasswordCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(command.Email);
            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            var isSuccessfull = PasswordHash.Validate(command.OldPassword, user.PasswordSalt, user.PasswordHash);
            _logger.LogInfo($"Change Password command validation Result{isSuccessfull}");
            if (!isSuccessfull)
                return Errors.Authentication.InvalidCredentials;

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(command.NewPassword, user.PasswordSalt);
            _userRepository.Update(user);

            await _emailService.SendEmailAsync(command.Email, "Password Changed", $"Successfully changed Password!");
            return true;
        }
    }
}