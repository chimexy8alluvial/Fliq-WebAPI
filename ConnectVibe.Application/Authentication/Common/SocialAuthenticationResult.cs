using ConnectVibe.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Authentication.Common
{
    public record SocialAuthenticationResult
    (
        User user,
       string Token,
       bool IsNewUser
    );
}