namespace Fliq.Application.BusinessDocumentType.Common
{
    public record BusinessDocumentTypeResponse
    {
        public int Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool HasFrontAndBack { get; init; }
    }
}
