namespace Fliq.Contracts.Games
{
    public record CreateGameDto(
      string Name,
      string? Description,
      bool RequiresLevel,
      bool RequiresTheme,
      bool RequiresCategory
  );
}