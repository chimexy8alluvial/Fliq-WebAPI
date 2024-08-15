﻿using ConnectVibe.Application.Authentication.Commands.Register;
using ConnectVibe.Application.Authentication.Commands.ValidateOTP;
using ConnectVibe.Application.Authentication.Queries.FacebookLogin;
using ConnectVibe.Application.Authentication.Queries.GoogleLogin;
using ConnectVibe.Application.Common.Interfaces.Services;
using ConnectVibe.Application.Authentication.Queries.Login;
using ConnectVibe.Application.Common.Interfaces.Persistence;
using ConnectVibe.Contracts.Authentication;
using ConnectVibe.Domain.Common.Errors;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ConnectVibe.Application.Authentication.Commands.ChangePassword;
using ConnectVibe.Application.Authentication.Commands.PasswordReset;
using ConnectVibe.Application.Authentication.Commands.ValidatePasswordOTP;
using ConnectVibe.Application.Authentication.Commands.PasswordCreation;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ConnectVibe.Api.Controllers
{
    [Route("api/auth")]
    [AllowAnonymous]
    public class AuthenticationController : ApiBaseController
    {
        private readonly ISender _mediator;
        private readonly IMapper _mapper;
        private readonly IOtpRepository _otpRepository;
        private readonly ILoggerManager _logger;
        public AuthenticationController(ISender mediator, IMapper mapper, IOtpRepository otpRepository, ILoggerManager logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _otpRepository = otpRepository;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            _logger.LogInfo($"------About the register the following user: ----{JsonConvert.SerializeObject(request)}");
            var command = _mapper.Map<RegisterCommand>(request);
            var authResult = await _mediator.Send(command);

            return authResult.Match(
                authResult => Ok(_mapper.Map<RegisterResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("validate-otp")]
        public async Task<IActionResult> ValidateOtp([FromBody] ValidateOtpRequest request)
        {
            var command = _mapper.Map<ValidateOTPCommand>(request);
            var authResult = await _mediator.Send(command);
            return authResult.Match(
                 authResult => Ok(_mapper.Map<AuthenticationResponse>(authResult)),
                 errors => Problem(errors)
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var command = _mapper.Map<LoginQuery>(request);
            var authResult = await _mediator.Send(command);

            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidCredentials)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<AuthenticationResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [Authorize]
        [HttpPost("changePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _logger.LogInfo($"------About to change Password for the following user: ----{JsonConvert.SerializeObject(request)}");
            var command = _mapper.Map<ChangePasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"------Change Password AuthResult: ----{JsonConvert.SerializeObject(authResult)}");

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
            _logger.LogInfo($"------Forgot Password Request: ----{JsonConvert.SerializeObject(request)}");
            var command = _mapper.Map<ForgotPasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"------ Forgot AuthResult: ----{JsonConvert.SerializeObject(authResult)}");

            return authResult.Match(
                authResult => Ok(_mapper.Map<ForgotPasswordResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("validateforgotpasswordotp")]
        public async Task<IActionResult> ValidateForgotPasswordPasswordOtp([FromBody] SendPasswordOTPRequest request)
        {
            _logger.LogInfo($"-----ValidatePassword_otp Request: ----{JsonConvert.SerializeObject(request)}");
            var command = _mapper.Map<ValidatePasswordOTPCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"------ ValidatePassword_otp AuthResult: ----{JsonConvert.SerializeObject(authResult)}");

            return authResult.Match(
                authResult => Ok(_mapper.Map<ValidatePasswordOTPResponse>(authResult)),
                errors => Problem(errors)
            );
        }

        [HttpPost("createPassword")]
        public async Task<IActionResult> CreatePassword([FromBody] NewPasswordRequest request)
        {
            _logger.LogInfo($"-----CreatePassword Request: ----{JsonConvert.SerializeObject(request)}");
            var command = _mapper.Map<CreatePasswordCommand>(request);
            var authResult = await _mediator.Send(command);
            _logger.LogInfo($"------ CreatePassword AuthResult: ----{JsonConvert.SerializeObject(authResult)}");

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
            var command = _mapper.Map<GoogleLoginQuery>(request);
            var authResult = await _mediator.Send(command);

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
            var command = _mapper.Map<FacebookLoginQuery>(request);
            var authResult = await _mediator.Send(command);

            if (authResult.IsError && authResult.FirstError == Errors.Authentication.InvalidToken)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, title: authResult.FirstError.Description);
            }

            return authResult.Match(
                authResult => Ok(_mapper.Map<SocialAuthenticationResponse>(authResult)),
                errors => Problem(errors)
                );
        }
    }
}