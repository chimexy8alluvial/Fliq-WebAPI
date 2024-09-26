using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.Profile
{
    public class Occupation
    {
        public int Id { get; set; }
        public string OccupationName { get; set; } = default!;
        public bool IsVisible { get; set; }

    }
}
