
using FirebaseAdmin.Messaging;

namespace Fliq.Infrastructure.Services.NotificationServices.Firebase
{
    public class FirebaseMessagingWrapper : IFirebaseMessagingWrapper
    {
        public async Task<BatchResponse> SendEachForMulticastAsync(MulticastMessage message)
        {
            return await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
        }
    }
}
