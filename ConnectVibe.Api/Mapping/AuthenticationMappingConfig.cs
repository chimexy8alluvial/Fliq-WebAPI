using Fliq.Application.Authentication.Commands.Register;
using Fliq.Application.Authentication.Common;
using Fliq.Application.Authentication.Queries.FacebookLogin;
using Fliq.Application.Authentication.Queries.GoogleLogin;
using Fliq.Application.Authentication.Queries.Login;
using Fliq.Contracts.Authentication;
using Mapster;

namespace Fliq.Api.Mapping
{
    public class AuthenticationMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<RegisterRequest, RegisterCommand>();
            config.NewConfig<LoginRequest, LoginQuery>();
            config.NewConfig<GoogleLoginRequest, GoogleLoginQuery>();
            config.NewConfig<FacebookLoginRequest, FacebookLoginQuery>();
            //the two configs above are not necessary because the properties are the same, the mapping happens automatically but we just add it here for reference purpose.
            config.NewConfig<AuthenticationResult, AuthenticationResponse>().
                Map(dest => dest.Token, src => src.Token).
                Map(dest => dest, src => src.user);
            config.NewConfig<SocialAuthenticationResult, SocialAuthenticationResponse>().
               Map(dest => dest.Token, src => src.Token).
               Map(dest => dest.IsNewUser, src => src.IsNewUser).
               Map(dest => dest, src => src.user);
            config.NewConfig<RegistrationResult, RegisterResponse>().
                Map(dest => dest.Otp, src => src.otp).
                Map(dest => dest, src => src.user);
            config.NewConfig<RegistrationResult, RegisterResponse>().
               Map(dest => dest.Otp, src => src.otp).
               Map(dest => dest, src => src.user);
            config.NewConfig<ValidatePasswordOTPResult, ValidatePasswordOTPResponse>().
       Map(dest => dest.otp, src => src.otp).
       Map(dest => dest, src => src.user);
            config.NewConfig<ForgotPasswordResult, ForgotPasswordResponse>().
       Map(dest => dest.otp, src => src.otp).
       Map(dest=>dest.email,src=>src.email);

        }
    }
}