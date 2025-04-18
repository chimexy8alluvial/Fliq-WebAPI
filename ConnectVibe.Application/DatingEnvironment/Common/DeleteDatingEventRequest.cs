using Fliq.Domain.Entities.Event.Enums;

namespace Fliq.Application.DatingEnvironment.Common
{
    public record DeleteDatingEventRequest(List<DatingOptions> DatingOptions);

    public class DatingOptions
    {
        public int id { get; set; }
        public DatingType DatingType { get; set; }
    }
}
