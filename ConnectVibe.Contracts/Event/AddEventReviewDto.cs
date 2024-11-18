namespace Fliq.Contracts.Event
{
    public record AddEventReviewDto(

       int EventId,
       int Rating,
       string Comments
       );
}