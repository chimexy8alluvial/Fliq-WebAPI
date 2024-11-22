using FirebaseAdmin.Messaging;

namespace Fliq.Infrastructure.Services.NotificationServices.Firebase
{
    public interface IFirebaseMessagingWrapper
    {
       Task<BatchResponse> SendEachForMulticastAsync(MulticastMessage message);
    }
}