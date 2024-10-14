namespace Fliq.Domain.Entities
{
    public class Record
    {
        public int Id { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateModified { get; set; }
        public bool IsDeleted { get; set; }
    }
}