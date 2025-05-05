
namespace Fliq.Domain.Entities.Profile
{
    public class BusinessIdentificationDocument : Record
    {
        public int UserProfileId { get; set; }
        public UserProfile? UserProfile { get; set; }
        public int BusinessIdentificationDocumentTypeId { get; set; }
        public BusinessIdentificationDocumentType? BusinessIdentificationDocumentType { get; set; }
        public string FrontDocumentUrl { get; set; } = string.Empty;
        public string? BackDocumentUrl { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
    }
}
