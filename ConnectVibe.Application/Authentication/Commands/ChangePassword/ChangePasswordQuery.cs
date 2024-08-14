using ConnectVibe.Application.Common.Interfaces.Authentication;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Common.Security;
using ConnectVibe.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;


namespace ConnectVibe.Application.Authentication.Commands.ChangePassword
{
    public record ChangePasswordQuery(
        string Email,
        string OldPassword,
        string NewPassword
        ) : IRequest<ErrorOr<bool>>;

    public class ChangePasswordQueryHandler : IRequestHandler<ChangePasswordQuery, ErrorOr<bool>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpRepository _otpRepository;
        private readonly IOtpService _otpService;
        public ChangePasswordQueryHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpRepository otpRepository, IOtpService otpService)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpRepository = otpRepository;
            _otpService = otpService;
        }
        public async Task<ErrorOr<bool>> Handle(ChangePasswordQuery query, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;
            var user = _userRepository.GetUserByEmail(query.Email);
            if (user == null)
                return Errors.Authentication.InvalidCredentials;

            var isSuccessfull = PasswordHash.Validate(query.OldPassword, user.PasswordSalt, user.PasswordHash);

            if (!isSuccessfull)
                return Errors.Authentication.InvalidCredentials;

            user.PasswordSalt = PasswordSalt.Create();
            user.PasswordHash = PasswordHash.Create(query.NewPassword, user.PasswordSalt);
            _userRepository.Update(user);

            await _emailService.SendEmailAsync(query.Email, "Password Changed", $"Successfully changed Password!");
            return true;
        }
    }
}
