

namespace Fliq.Domain.Enums
{
    public enum DateStatus
    {
        Pending = 0, 
        Ongoing = 1, 
        Completed = 2, 
        Cancelled = 3  
    }

    public enum DateParticipantStatus
    {
        Joined = 0,
        Left,
        Disconnected
    }
}
