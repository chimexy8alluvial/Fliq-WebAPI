using Fliq.Contracts.Profile;

namespace ConnectVibe.Contracts.Profile
{
    public record CreateProfileRequest(
    int UserId,
     DateTime DOB,
      List<string> Passions,
    List<ProfilePhotoDto> Photos,
    List<ProfileTypeDto> profileTypes,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     EthnicityDto Ethnicity,
     OccupationDto Occupation,
     EducationStatusDto EducationStatus,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     bool AllowNotifications = false

 );
}