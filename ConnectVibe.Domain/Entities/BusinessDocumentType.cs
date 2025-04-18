namespace Fliq.Domain.Entities
{
    public class BusinessDocumentType : Record
    {
        public string Name { get; set; } = string.Empty;
        public bool HasFrontAndBack { get; set; } = false;
    }
}
