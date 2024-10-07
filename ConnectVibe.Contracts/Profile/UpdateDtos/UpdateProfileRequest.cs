﻿namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateProfileRequest(
     int UserId,
     DateTime? DOB,
      List<string>? Passions,
    List<UpdateProfilePhotoDto>? Photos,
    List<UpdateProfileTypeDto>? profileTypes,
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