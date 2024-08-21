namespace ConnectVibe.Contracts.Profile
{
    public record CreateProfileRequest(
    int UserId,
     DateTime DOB,
      List<string> Passions,
    List<ProfilePhotoDto> Photos,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     EthnicityDto Ethnicity,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     bool AllowNotifications = false

 );
}