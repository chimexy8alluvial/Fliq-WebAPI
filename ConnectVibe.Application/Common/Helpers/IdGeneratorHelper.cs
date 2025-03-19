namespace Fliq.Application.Common.Helpers
{
    public static class IdGeneratorHelper
    {
        public static string GenerateSupportTicketId()
        {
            return $"TICKET-{Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()}";
        }
    }
}