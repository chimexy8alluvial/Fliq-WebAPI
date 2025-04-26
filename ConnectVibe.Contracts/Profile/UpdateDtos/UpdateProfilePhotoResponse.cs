namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateProfilePhotoResponse(string Caption, string PictureUrl);

    public record ReadProfilePhotoResponse(string Caption, string PictureUrl);
}