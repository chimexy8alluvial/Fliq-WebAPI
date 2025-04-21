namespace Fliq.Domain.Entities
{
    public class BusinessIdentificationDocumentType : Record
    {
        public string Name { get; set; } = string.Empty;
        public bool HasFrontAndBack { get; set; } = false;
    }
}
