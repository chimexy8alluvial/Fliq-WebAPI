namespace Fliq.Contracts.Event
{
    public record EventCriteriaDto
    (
        Event_TypeDto EventType,
        GenderDto Gender,
        List<string> Race
    );
}
