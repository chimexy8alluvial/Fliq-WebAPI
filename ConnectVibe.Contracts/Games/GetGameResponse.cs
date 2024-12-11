namespace Fliq.Contracts.Games
{
    public record GetGameResponse(
    int Id,
    string Name,
    string? Description,
    bool RequiresLevel,
    bool RequiresTheme,
    bool RequiresCategory,
    int Status
);
}