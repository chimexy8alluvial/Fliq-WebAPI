namespace Fliq.Domain.Entities.Profile
{
    public class ProfilePhoto : Record
    {
        public string PictureUrl { get; set; } = default!;
        public string Caption { get; set; } = default!;
    }
}