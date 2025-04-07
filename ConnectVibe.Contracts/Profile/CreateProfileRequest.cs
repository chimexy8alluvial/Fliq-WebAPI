namespace Fliq.Contracts.Profile
{
    public record CreateProfileRequest(
    DateTime? DOB,
    List<string>? Passions,
    List<ProfilePhotoDto>? Photos,
    List<ProfileTypeDto>? ProfileTypes,
    string? ProfileDescription,
    Gender? GenderId,
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

    public enum Gender
    {
        Male=1,
        Female,
        Both
    };
    public enum SexualOrientationType
    {
        Men=1,
        Women,
        Both
    }
    public enum HaveKidsType
    {
        Yes=1,
        No,
        PreferNotToSay
    }
    public enum WantKidsType
    {
        Yes=1,
        No,
        PreferNotToSay
    }
}