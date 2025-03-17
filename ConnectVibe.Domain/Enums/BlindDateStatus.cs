

namespace Fliq.Domain.Enums
{
    public enum BlindDateStatus
    {
        Pending = 0, 
        Ongoing = 1, 
        Completed = 2, 
        Cancelled = 3  
    }

    public enum BlindDateParticipantStatus
    {
        Joined = 0,
        Left,
        Disconnected
    }
}
