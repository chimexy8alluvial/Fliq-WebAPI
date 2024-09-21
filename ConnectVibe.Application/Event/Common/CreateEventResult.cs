using ConnectVibe.Domain.Entities.Profile;
using Fliq.Domain.Entities.Event;
using Fliq.Domain.Entities.Event.Enums;
namespace Fliq.Application.Event.Common
{
    public record CreateEventResult
    (
        Events Events
    );
}
