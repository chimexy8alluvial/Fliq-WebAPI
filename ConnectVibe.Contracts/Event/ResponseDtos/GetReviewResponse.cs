namespace Fliq.Contracts.Event.ResponseDtos
{
    public record GetReviewResponse(
        int Id,
        int UserId,
        int EventId,
        string Comment,
        int Rating);
}