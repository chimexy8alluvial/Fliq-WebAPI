using Fliq.Application.Authentication.Common;
using Fliq.Application.Common.Interfaces.Authentication;
using Fliq.Application.Common.Interfaces.Persistence;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Domain.Common.Errors;
using ErrorOr;
using MapsterMapper;
using MediatR;
using StreamChat.Models;
using StreamChat.Clients;

namespace Fliq.Application.Authentication.Commands.ValidateOTP
{
    public record ValidateOTPCommand(
        string Email,
        string Otp
        ) : IRequest<ErrorOr<AuthenticationResult>>;

    public class ValidateOTPCommandHandler : IRequestHandler<ValidateOTPCommand, ErrorOr<AuthenticationResult>>
    {
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        private readonly ILoggerManager _logger;
        private readonly StreamClientFactory _streamClientFactory;

        public ValidateOTPCommandHandler(IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository, IMapper mapper, IEmailService emailService, IOtpService otpService, ILoggerManager logger, StreamClientFactory streamClientFactory)
        {
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
            _mapper = mapper;
            _emailService = emailService;
            _otpService = otpService;
            _logger = logger;
            _streamClientFactory = streamClientFactory;
        }

        public async Task<ErrorOr<AuthenticationResult>> Handle(ValidateOTPCommand command, CancellationToken cancellationToken)
        {
            await Task.CompletedTask;

            if (!await _otpService.ValidateOtpAsync(command.Email, command.Otp))
                return Errors.Authentication.InvalidOTP;
            var user = _userRepository.GetUserByEmail(command.Email);
            user.IsEmailValidated = true;
            user.IsActive = true;
            _userRepository.Update(user);
            var token = _jwtTokenGenerator.GenerateToken(user);

            #region Stream Api implementation
            // Generate Stream Chat token
            var userClient = _streamClientFactory.GetUserClient();
            var streamToken = userClient.CreateToken(user.Id.ToString()); // Use user ID or username

            //sync user with stream
            var streamUser = new UserRequest
            {
                Id = user.Id.ToString(),
                Role = user.Role.Name,
            };

            streamUser.SetData("email", user.Email);
            streamUser.SetData("name", user.DisplayName);

            await userClient.UpsertManyAsync([streamUser]);
            #endregion

            return new AuthenticationResult(user, token, streamToken);
        }
    }
}