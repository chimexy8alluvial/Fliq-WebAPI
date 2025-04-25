namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateReligionDto
    (int Id, int? ReligionType, bool? IsVisible);

    public record ReadReligionDto
    (int Id, string? ReligionType, bool? IsVisible);
}