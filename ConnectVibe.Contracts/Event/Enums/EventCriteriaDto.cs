using Fliq.Contracts.Event.Enum;

namespace Fliq.Contracts.Event.Enums
{
    public record EventCriteriaDto
    (
        Event_TypeDto EventType,
        GenderDto Gender,
        string Race
    );
}
