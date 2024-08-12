using ConnectVibe.Application.Authentication.Queries.GoogleLogin;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Authentication.Queries.FacebookLogin
{
    public class FacebookLoginQueryValidator : AbstractValidator<FacebookLoginQuery>
    {
        public FacebookLoginQueryValidator()
        {
            RuleFor(x => x.Code).NotEmpty()
                .WithMessage("Code is required");
        }
    }
}