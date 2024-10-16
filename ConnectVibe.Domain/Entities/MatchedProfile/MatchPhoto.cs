using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.MatchedProfile
{
    public class MatchPhoto
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; } = default!;
        public string Caption { get; set; } = default!;
    }
}
