using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Application.Profile.Common
{
    public class ProfilePhotoMapped
    {
        public string Caption { get; set; } = default!;

        public IFormFile ImageFile { get; set; } = default!;
    }
}