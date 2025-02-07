namespace Fliq.Domain.Entities
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public DateTime? DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}