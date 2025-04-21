using Fliq.Contracts.Enums;

namespace Fliq.Contracts.Profile
{
    public record CreateProfileRequest(
    DateTime? DOB,
    List<string>? Passions,
    List<ProfilePhotoDto>? Photos,
    BusinessIdentificationDocumentDto BusinessIdentificationDocument,
    List<ProfileTypeDto>? ProfileTypes,
    string? ProfileDescription,
    GenderType? GenderId,
    SexualOrientationType? SexualOrientationId,
    bool IsSexualOrientationVisible,
    int? ReligionId,
    bool IsReligionVisible,
    int? EthnicityId,
    bool IsEthnicityVisible,
    int? OccupationId,
    bool? IsOccupationVisible,
    int? EducationStatusId,
    bool? IsEducationStatusVisible,
    HaveKidsType? HaveKidsId,
    WantKidsType? WantKidsId,
    LocationDto? Location,
    List<CreatePromptResponseDto>? PromptResponses,
    int? CurrentSection,
    bool AllowNotifications = false
);


}