namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateOccupationDto
        (int Id, int? OccupationName, bool? IsVisible);

    public record ReadOccupationDto
       (int Id, string? OccupationName, bool? IsVisible);
}