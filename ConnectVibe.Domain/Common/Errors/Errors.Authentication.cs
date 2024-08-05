﻿using ErrorOr;

namespace ConnectVibe.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Authentication
        {
            public static Error InvalidCredentials => Error.Validation(
            code: "User.InvalidCredentials",
            description: "Invalid login credentials");
        }
    }
}
