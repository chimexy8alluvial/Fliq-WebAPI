namespace Fliq.Domain.Entities.Profile
{
    public class BusinessIdentificationDocument : Record
    {
        public int UserId { get; set; }
        public User? User { get; set; }
        public int BusinessDocumentTypeId { get; set; }
        public BusinessDocumentType? BusinessDocumentType { get; set; }
        public string FrontDocumentUrl { get; set; }
        public string? BackDocumentUrl { get; set; }
        public DateTime UploadedDate { get; set; } = DateTime.UtcNow;
        public bool IsVerified { get; set; } = false;
        public DateTime? VerifiedDate { get; set; }
    }
}
