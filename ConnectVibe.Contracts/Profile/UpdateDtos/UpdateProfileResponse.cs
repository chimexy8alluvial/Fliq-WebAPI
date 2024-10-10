namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateProfileResponse
    (int UserId,
     DateTime DOB,
     UpdateGenderDto Gender,
     UpdateSexualOrientationDto SexualOrientation,
     UpdateReligionDto Religion,
     UpdateEthnicityDto Ethnicity,
     UpdateHaveKidsDto HaveKids,
     UpdateWantKidsDto WantKids,
     UpdateLocationDto Location,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<UpdateProfilePhotoResponse> Photos = default!);
}