using Fliq.Contracts.Prompts;

namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateProfileResponse
    (int UserId,
     DateTime DOB,
     UpdateGenderDto Gender,
     UpdateSexualOrientationDto SexualOrientation,
     UpdateReligionDto Religion,
     UpdateEthnicityDto Ethnicity,
     UpdateEducationStatusDto EducationStatus,
     UpdateHaveKidsDto HaveKids,
     UpdateWantKidsDto WantKids,
     UpdateLocationDto Location,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<UpdateProfilePhotoResponse> Photos = default!,
        List<PromptResponseDto> PromptResponses = default!);

    public record ReadProfileResponse
   (int UserId,
    DateTime DOB,
    ReadGenderDto Gender,
    ReadSexualOrientationDto SexualOrientation,
    ReadReligionDto Religion,
    ReadEthnicityDto Ethnicity,
    ReadEducationStatusDto EducationStatus,
    ReadHaveKidsDto HaveKids,
    ReadWantKidsDto WantKids,
    ReadLocationDto Location,
    bool AllowNotifications = false,
    List<string> Passions = default!,
   List<ReadProfilePhotoResponse> Photos = default!,
       List<ReadPromptResponseDto> PromptResponses = default!);
}