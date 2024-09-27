namespace Fliq.Domain.Entities.Profile
{
    public class ProfilePhoto
    {
        public int Id { get; set; }
        public string PictureUrl { get; set; } = default!;
        public string Caption { get; set; } = default!;
    }
}