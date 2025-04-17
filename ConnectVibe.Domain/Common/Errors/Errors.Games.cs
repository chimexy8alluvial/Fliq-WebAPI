using ErrorOr;

namespace Fliq.Domain.Common.Errors
{
    public static partial class Errors
    {
        public static class Games
        {
            public static Error GameNotFound => Error.NotFound(
                code: "Games.Game.NotFound",
                description: "Game with the given ID was not found.");

            public static Error InvalidGameState => Error.Conflict(
                code: "Games.Game.InvalidState",
                description: "The game is in an invalid state for the requested action.");

            public static Error GameRequestAlreadySent => Error.Conflict(
                code: "Games.Request.AlreadySent",
                description: "A request for this game has already been sent to the player.");

            public static Error GameRequestNotFound => Error.NotFound(
                code: "Games.Request.NotFound",
                description: "Game request with the given ID was not found.");

            public static Error GameRequestAlreadyProcessed => Error.Conflict(
                code: "Games.Request.AlreadyProcessed",
                description: "The game request has already been processed.");

            public static Error NotYourTurn => Error.Conflict(
                code: "Games.Turn.NotYourTurn",
                description: "It is not your turn to play in this game.");

            public static Error NoActiveQuestion => Error.Conflict(
                code: "Games.Question.NoActiveQuestion",
                description: "There is no active question to answer in this game.");

            public static Error QuestionNotFound => Error.NotFound(
                code: "Games.Question.NotFound",
                description: "The specified question was not found.");

            public static Error InvalidAnswer => Error.Validation(
                code: "Games.Answer.Invalid",
                description: "The provided answer is invalid or in an unsupported format.");

            public static Error GameSessionNotFound => Error.NotFound(
                code: "Games.Session.NotFound",
                description: "Game session with the given ID was not found.");

            public static Error GameSessionCompleted => Error.Conflict(
                code: "Games.Session.Completed",
                description: "The game session has already been completed.");

            public static Error InvalidGameConfiguration => Error.Validation(
                code: "Games.Configuration.Invalid",
                description: "The game configuration is invalid or incomplete.");

            public static Error NoGamesFound => Error.NotFound(
                code: "Games.Game.NotFound",
                description: "No games found matching the provided filters.");
        }
    }
}