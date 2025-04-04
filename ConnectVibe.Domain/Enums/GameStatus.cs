namespace Fliq.Domain.Enums
{
    public enum GameStatus
    {
        Pending = 0,
        Accepted = 1,
        Rejected = 2,
        InProgress = 3,
        Done = 4
    }

    public enum GameCreationStatus
    {
        Pending = 0,
        Approved = 1,
        Rejected = 2,
    }
}