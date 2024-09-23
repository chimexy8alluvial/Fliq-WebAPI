using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Fliq.Application.Authentication.Commands.ValidatePasswordOTP
{
    public class ValidatePasswordOTPValidator : AbstractValidator<ValidatePasswordOTPCommand>
    {
        public ValidatePasswordOTPValidator()
        {
            RuleFor(x => x.Otp).NotEmpty();
            RuleFor(x => x.Email).NotEmpty().EmailAddress().WithMessage("Invalid email format.");
        }
    }
}
