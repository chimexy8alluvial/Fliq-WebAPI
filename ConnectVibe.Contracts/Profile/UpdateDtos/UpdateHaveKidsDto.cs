namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateHaveKidsDto
    (int Id, int? HaveKidsType, bool? IsVisible);

    public record ReadHaveKidsDto
   (int Id, string? HaveKidsType, bool? IsVisible);
}