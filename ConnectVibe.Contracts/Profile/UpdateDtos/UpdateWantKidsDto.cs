namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateWantKidsDto(int Id, int? WantKidsType, bool? IsVisible);
    public record ReadWantKidsDto(int Id, string? WantKidsType, bool? IsVisible);
}