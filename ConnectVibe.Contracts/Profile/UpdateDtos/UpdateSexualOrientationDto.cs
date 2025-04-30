namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateSexualOrientationDto
    (int Id, int? SexualOrientationType, bool? IsVisible);

    public record ReadSexualOrientationDto
    (int Id, string? SexualOrientationType, bool? IsVisible);
}