namespace Fliq.Contracts.Profile.UpdateDtos
{
    public record UpdateEthnicityDto(int Id, int? EthnicityType, bool? IsVisible);

    public record ReadEthnicityDto(int Id, string? EthnicityType, bool? IsVisible);
}