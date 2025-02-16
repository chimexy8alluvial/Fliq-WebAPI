

namespace Fliq.Domain.Entities
{
    public class Role : Record
    {
        public string Name { get; set; } = string.Empty;
        public ICollection<User>? Users { get; set; }
    }
}
