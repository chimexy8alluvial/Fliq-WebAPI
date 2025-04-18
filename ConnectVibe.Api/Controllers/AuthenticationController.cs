using Fliq.Application.Authentication.Business.Command.Register;
using Fliq.Application.Authentication.Commands.ChangePassword;
using Fliq.Application.Authentication.Commands.PasswordCreation;
using Fliq.Application.Authentication.Commands.PasswordReset;
using Fliq.Application.Authentication.Commands.Register;
using Fliq.Application.Authentication.Commands.ValidateOTP;
using Fliq.Application.Authentication.Commands.ValidatePasswordOTP;
using Fliq.Application.Authentication.Queries.FacebookLogin;
using Fliq.Application.Authentication.Queries.GoogleLogin;
using Fliq.Application.Authentication.Queries.Login;
using Fliq.Application.Authentication.Queries.SetupData;
using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Profile.Common;
using Fliq.Contracts.Authentication;
using Fliq.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Fliq.Api.Controllers
{
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticationController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly ILoggerManager _logger;

        public AuthenticationController(ISender mediator, IMapper mapper, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInfo($"Register Request Received: {request}");
            var command = _mapper.Map<RegisterCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Register Command Executed. Result: {authResult}");
            return authResult.Match(
                authResult => Ok(_mapper.Map<RegisterResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("register-business")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> RegisterBusiness([FromForm] RegisterBusinessRequest request)
        {
            _logger.LogInfo($"Register Business Request Received: {request}");
            var command = _mapper.Map<RegisterBusinessCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Register Business Command Executed. Result: {authResult}");
            return authResult.Match(
                authResult => Ok(_mapper.Map<RegisterBusinessResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request)
        {
            _logger.LogInfo($"Validate-otp Request Received: {request}");
            var command = _mapper.Map<ValidateOTPCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Validate-otp Command Executed. Result: {authResult}");
            return authResult.Match(
                 authResult => Ok(_mapper.Map<AuthenticationResponse>(authResult)),
                 errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            _logger.LogInfo($"Login Request Received: {request}");
            var command = _mapper.Map<LoginQuery>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Login Command Executed.  Result: {authResult}");
            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }
            _logger.LogInfo($"Login command executed with error.  Result: {authResult}");
            return authResult.Match(
                authResult => Ok(_mapper.Map<AuthenticationResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _logger.LogInfo($"ChangePassword Request Received: {request}");
            var command = _mapper.Map<ChangePasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Change Password Command Executed.  Result: {authResult}");

            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<ChangePasswordResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("forgetPassword")]
        public async Task<IActionResult> ForgetPassword([FromBody] ForgotPasswordRequest request)
        {
            _logger.LogInfo($"Forgot Password Request Received: {request}");
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Forgot Password Command Executed.  Result: {authResult}");

            return authResult.Match(
                authResult => Ok(_mapper.Map<ForgotPasswordResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("validateforgot-password-otp")]
        public async Task<IActionResult> ValidateForgotPasswordOtp([FromBody] SendPasswordOTPRequest request)
        {
            _logger.LogInfo($"Validate_Forgot_Password_otp Request Received: {request}");
            var command = _mapper.Map<ValidatePasswordOTPCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Validate_Forgot_Password_otp Command Execute. Result: {authResult}");

            return authResult.Match(
                authResult => Ok(_mapper.Map<ValidatePasswordOTPResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("createPassword")]
        public async Task<IActionResult> CreatePassword([FromBody] NewPasswordRequest request)
        {
            _logger.LogInfo($"CreatePassword Request Received: {request}");
            var command = _mapper.Map<CreatePasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"CreatePassword Command Executed.  Result: {authResult}");

            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<bool>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("Login/google")]
        public async Task<IActionResult> GoogleLogin([FromQuery] GoogleLoginRequest request)
        {
            _logger.LogInfo($"Google Login Request Received: {request}");
            var command = _mapper.Map<GoogleLoginQuery>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"Google Login Command Executed.  Result: {authResult}");
            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidToken)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<SocialAuthenticationResponse>(authResult)),
                errors => Problem(errors)
                );
        }

        [HttpPost("Login/facebook")]
        public async Task<IActionResult> FacebookLogin([FromQuery] FacebookLoginRequest request)
        {
            _logger.LogInfo($"Facebook Login Request Received: {request}");
            var command = _mapper.Map<FacebookLoginQuery>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"FacebookLogin Command Executed.  Result: {authResult}");
            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidToken)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<SocialAuthenticationResponse>(authResult)),
                errors => Problem(errors)
                );
        }

        [HttpGet("getprofilesetupdata")]
        public async Task<IActionResult> GetProfileSetupData()
        {
            _logger.LogInfo("Received request for Profile Setup Data");

            var query = new GetProfileSetupDataQuery();
            var result = await _mediator.Send(query);

            return result.Match(
               result => Ok(_mapper.Map<ProfileDataTablesResponse>(result)),
               errors => Problem(errors)
               );
        }
    }
}