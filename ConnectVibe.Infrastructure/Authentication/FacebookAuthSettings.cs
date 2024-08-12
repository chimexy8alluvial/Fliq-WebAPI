using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Infrastructure.Authentication
{
    public class FacebookAuthSettings
    {
        public static string SectionName = "FacebookAuthSettings";

        public string TokenValidationUrl { get; set; }
        public string UserInfoUrl { get; set; }
        public string AppId { get; set; }
        public string AppSecret { get; set; }
    }
}