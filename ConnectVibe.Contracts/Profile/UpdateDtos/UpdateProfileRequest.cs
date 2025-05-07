namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateProfileRequest(
     DateTime? DOB,
      List<string>? Passions,
    List<UpdateProfilePhotoDto>? Photos,
    List<UpdateProfileTypeDto>? profileTypes,
    UpdateBusinessIdentificationDocumentDto? BusinessIdentificationDocument,
    string? ProfileDescription,
     UpdateGenderDto? Gender,
     UpdateSexualOrientationDto? SexualOrientation, 
     UpdateReligionDto? Religion,
     UpdateEthnicityDto? Ethnicity,
     UpdateOccupationDto? Occupation,
     UpdateEducationStatusDto? EducationStatus,
     UpdateHaveKidsDto? HaveKids,
     UpdateWantKidsDto? WantKids,
     UpdateLocationDto? Location,
     bool? AllowNotifications = false

 );
}