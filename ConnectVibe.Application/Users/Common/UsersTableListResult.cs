namespace Fliq.Application.Users.Common
{
    public class UsersTableListResult 
    {
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public string Email { get; set; } = default!;
        public string Subscription {  get; set; } = default!;
        public DateTime? DateCreated { get; set; }
        public DateTime? LastActiveAt { get; set; }
        
    }
}
