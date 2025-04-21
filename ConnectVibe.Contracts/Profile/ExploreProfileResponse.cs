
namespace Fliq.Contracts.Profile
{
    public record ExploreProfileResponse
    (int UserId,
     DateTime DOB,
     GenderDto Gender,
     SexualOrientationDto SexualOrientation,
     ReligionDto Religion,
     OccupationDto Occupation,
     EducationStatusDto EducationStatus,
     List<ProfileTypeDto> ProfileTypes,
     BusinessIdentificationDocumentResponse BusinessIdentificationDocument,
     EthnicityDto Ethnicity,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<ProfilePhotoResponse> Photos = default!,
        List<ExplorePromptResponseDto> PromptResponses = default!);

    public record ExplorePromptResponseDto(

     int PromptQuestionId,
     string? Response
     );
}