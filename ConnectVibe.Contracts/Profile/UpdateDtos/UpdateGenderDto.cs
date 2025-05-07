namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateGenderDto
    (int Id, int? GenderType, bool? IsVisible);

    public record ReadGenderDto
    (int Id, string? GenderType, bool? IsVisible);
}