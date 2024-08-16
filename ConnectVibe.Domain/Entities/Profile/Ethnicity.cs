namespace ConnectVibe.Domain.Entities.Profile
{
    public class Ethnicity
    {
        public int Id { get; set; }
        public EthnicityType EthnicityType { get; set; }
        public bool IsVisible { get; set; } = false;
    }

    public enum EthnicityType
    {
        AmericanOrIndian,
        BlackOrAfricanDescent,
        HispaniOrLatino,
        MiddleEastern,
        EastAsian,
        PacificIslander,
        SouthAsian,
        WhiteOrCaucasian,
        Other,
        PreferNotToSay
    }
}