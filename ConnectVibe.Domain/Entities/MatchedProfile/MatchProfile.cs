using ConnectVibe.Domain.Entities.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fliq.Domain.Entities.MatchedProfile
{
    public class MatchProfile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public GenderType GenderType { get; set; } = default!;
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string UserName {  get; set; } = default!;
        public int Age { get; set; }
        public string MatchStatus { get; set; }=default!;
        public int RequestingUserId { get; set; }
        public string RequestingUserEmail { get; set; } = default!;  
        public MatchPhoto Images { get; set; } = default!;
        public DateTime RequestTime { get; set; } = default!;
    }
}
