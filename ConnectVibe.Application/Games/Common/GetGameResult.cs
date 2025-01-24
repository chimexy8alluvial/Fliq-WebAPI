using Fliq.Domain.Entities.Games;

namespace Fliq.Application.Games.Common
{
    public record GetGameResult(
         int Id,
    string Name,
    string? Description,
    bool RequiresLevel,
    bool RequiresTheme,
    bool RequiresCategory
        );
}