using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Contracts.Profile
{
    public record ProfilePhotoResponse(string Caption, string Url);
}