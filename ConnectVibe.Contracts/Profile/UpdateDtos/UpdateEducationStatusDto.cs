namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateEducationStatusDto
        (int Id, int? EducationLevel, bool? IsVisible);

    public record ReadEducationStatusDto
        (int Id, string? EducationLevel, bool? IsVisible);
}