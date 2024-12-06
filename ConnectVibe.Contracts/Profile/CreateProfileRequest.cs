namespace Fliq.Contracts.Profile
{
    public record CreateProfileRequest(
     DateTime DOB,
      List<string> Passions,
    List<ProfilePhotoDto> Photos,
    List<ProfileTypeDto> ProfileTypes,
    string ProfileDescription,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     EthnicityDto Ethnicity,
     OccupationDto Occupation,
     EducationStatusDto EducationStatus,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     List<CreatePromptResponseDto> PromptResponses,
     bool AllowNotifications = false

 );
}