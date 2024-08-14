using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ConnectVibe.Domain.Entities;

namespace ConnectVibe.Application.Authentication.Common
{
    public record ValidatePasswordOTPResult
    (
        bool Response
    );
}
