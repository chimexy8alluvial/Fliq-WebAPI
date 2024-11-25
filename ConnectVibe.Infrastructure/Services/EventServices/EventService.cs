using Fliq.Application.Common.Interfaces.Services;
using Fliq.Application.Common.Interfaces.Services.EventServices;
using Fliq.Infrastructure.Event;
using Microsoft.Extensions.Options;

namespace Fliq.Infrastructure.Services.EventServices
{
    public class EventService : IEventService
    {
        private readonly EventSettings _eventSettings;
        private readonly IEmailService _emailService;

        public EventService(IOptions<EventSettings> eventSettings, IEmailService emailService)
        {
            _eventSettings = eventSettings.Value;
            _emailService = emailService;
        }

        public string GenerateEventCreationEmailContent(int eventId, string? name, bool isExternal = false)
        {
            var eventUrl = isExternal
                ? $"https://example.com/register?eventId={eventId}"
                : $"https://example.com/events/{eventId}";

            return $@"
        Hi {name},
        <p>You are invited to an event!</p>
        <p>Click <a href='{eventUrl}'>here</a> to view the event details.</p>
        {(isExternal ? "<p>If you're new, please sign up to join.</p>" : "")}
        Regards,<br>The Events Team
    ";
        }
    }
}