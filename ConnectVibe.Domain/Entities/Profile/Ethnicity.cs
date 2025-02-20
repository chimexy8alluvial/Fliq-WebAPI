namespace Fliq.Domain.Entities.Profile
{
    public class Ethnicity : Record
    {
        public int UserProfileId { get; set; }
        public EthnicityType EthnicityType { get; set; }
        public bool IsVisible { get; set; }
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