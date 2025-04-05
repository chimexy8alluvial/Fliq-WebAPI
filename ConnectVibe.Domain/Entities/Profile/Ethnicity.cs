namespace Fliq.Domain.Entities.Profile
{
    public class Ethnicity : Record
    {
        public string EthnicityType { get; set; } = default!;
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