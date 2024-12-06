namespace Fliq.Application.Common.Interfaces.Services.EventServices
{
    public interface IEventService
    {
        string GenerateEventCreationEmailContent(int eventId, string? name, bool isExternal = false);
    }
}