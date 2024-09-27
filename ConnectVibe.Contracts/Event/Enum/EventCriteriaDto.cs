namespace Fliq.Contracts.Event.Enum
{
    public record EventCriteriaDto
    (
        Event_TypeDto EventType,
        GenderDto Gender,
        string Race
    );
}
