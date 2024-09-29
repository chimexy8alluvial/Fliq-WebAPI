﻿namespace Fliq.Contracts.Profile
{
    public record ProfileResponse
    (int UserId,
     string FirstName,
     string LastName,
     string DisplayName,
     DateTime DOB,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     OccupationDto Occupation,
     EducationStatusDto EducationStatus,
     List<ProfileTypeDto> ProfileTypes,
     EthnicityDto Ethnicity,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<ProfilePhotoResponse> Photos = default!);
}