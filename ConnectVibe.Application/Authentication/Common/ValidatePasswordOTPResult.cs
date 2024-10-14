using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fliq.Domain.Entities;

namespace Fliq.Application.Authentication.Common
{
    public record ValidatePasswordOTPResult
    (
        User user,
        string otp
    );
}
