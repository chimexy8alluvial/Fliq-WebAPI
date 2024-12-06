namespace Fliq.Domain.Entities
{
    public class AgeRange : Record
    {
        public int MinAge { get; set; }
        public int MaxAge { get; set; }

        public AgeRange(int minAge, int maxAge)
        {
            MinAge = minAge;
            MaxAge = maxAge;
        }
    }
}