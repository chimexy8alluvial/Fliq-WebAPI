using Fliq.Contracts.Prompts;

namespace Fliq.Contracts.Profile
{
    public record ProfileResponse
    (int UserId,
     DateTime DOB,
     GenderDto Gender,
     int SexualOrientationId,
     int ReligionId,
     int OccupationId,
     int EducationStatusId,
     List<ProfileTypeDto> ProfileTypes,
     BusinessIdentificationDocumentResponse BusinessIdentificationDocument,
     int EthnicityId,
     HaveKidsDto HaveKids,
     WantKidsDto WantKids,
     LocationDto Location,
     bool AllowNotifications = false,
     List<string> Passions = default!,
    List<ProfilePhotoResponse> Photos = default!,
    List<PromptResponseDto> PromptResponses = default!,
    List<string> CompletedSections = default!
    );
}