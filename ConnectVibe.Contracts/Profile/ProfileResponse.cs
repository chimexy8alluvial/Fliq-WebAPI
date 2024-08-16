using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConnectVibe.Contracts.Profile
{
    public record ProfileResponse
    (int UserId,
     DateTime DOB,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     EthnicityDto Ethnicity,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     bool ShareLocation = default!,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<ProfilePhotoResponse> Photos = default!);
}