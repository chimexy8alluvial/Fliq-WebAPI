

namespace Fliq.Domain.Enums
{
    public enum BlindDateStatus
    {
        Pending = 0, 
        Ongoing, 
        Completed, 
        Cancelled  
    }

    public enum BlindDateParticipantStatus
    {
        Joined = 0,
        Left,
        Disconnected
    }
}
