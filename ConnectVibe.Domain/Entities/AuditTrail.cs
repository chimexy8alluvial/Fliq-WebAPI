
namespace Fliq.Domain.Entities
{
    public class AuditTrail : Record
    {
        public int UserId { get; set; }
        public string UserFirstName { get; set; } = string.Empty;
        public string UserLastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string AuditAction { get; set; } = string.Empty;
        public string UserRole { get; set; } = string.Empty ;
        public string IPAddress { get; set; } = string.Empty;
    }
}
