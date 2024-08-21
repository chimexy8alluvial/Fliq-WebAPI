using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Contracts.Profile
{
    public class LocationDto
    {
        public string Address { get; set; } = default!;
        public string PostCode { get; set; } = default!;
        public string Country { get; set; } = default!;
        public bool IsVisible { get; set; }
    }
}